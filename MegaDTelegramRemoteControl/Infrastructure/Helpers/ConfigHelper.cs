using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Scheduler;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;
using LocationItems = MegaDTelegramRemoteControl.Models.Device.LocationItems;
using LocationCondition = MegaDTelegramRemoteControl.Models.Device.LocationCondition;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers;

public static class ConfigHelper
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static HomeConfig MakeConfig(DevicesConfig devicesConfig, HomeMapConfig homeMapConfig,
        AutomationConfig automationConfig, KeeneticProxyConfig keeneticProxyConfig)
    {
        var devices = GetDevices(devicesConfig, automationConfig);
        var locations = GetLocations(homeMapConfig, devices);
        var schedulers = GetSchedulers(devices, automationConfig);

        return new HomeConfig
               {
                   Devices = devices,
                   Locations = locations,
                   Schedulers = schedulers,
               };
    }

    private static Dictionary<string, Device> GetDevices(DevicesConfig devicesConfig, AutomationConfig automationConfig)
    {
        var result = new Dictionary<string, Device>();

        foreach (var device in devicesConfig.Devices)
        {
            var deviceModel = new Device(device.Id, device.Name, device.Ip, device.Pwd);

            foreach (var port in device.DevicePorts)
            {
                deviceModel.Ports[port.Id] = new DevicePort
                                             {
                                                 Id = port.Id,
                                                 Name = port.Name,
                                                 Type = port.Type,
                                                 OutMode = port.OutMode,
                                                 InOutSWModeIcons = port.InOutSWModeIcons,
                                                 Device = deviceModel,
                                             };
            }

            result[device.Id] = deviceModel;
        }

        foreach (var trigger in automationConfig.Triggers)
        {
            if (!TryGetDevicePort(result, trigger.SourcePortState.DeviceId, trigger.SourcePortState.PortId, out var port))
            {
                logger.Warn($"Can't find source device/port for trigger {trigger}");
                continue;
            }

            var portTrigger = new DevicePortTriggerRule
                              {
                                  SourcePortStatus = trigger.SourcePortState.Status,
                                  Result = trigger.Result,
                                  IsFinal = trigger.IsFinal,
                              };

            foreach (var destinationPortState in trigger.DestinationPortStates)
            {
                if (!TryGetDevicePort(result, destinationPortState.DeviceId, destinationPortState.PortId, out var destinationPort))
                {
                    logger.Warn($"Can't find destination device/port for trigger {trigger}");
                    continue;
                }

                if (destinationPort.Type != DevicePortType.OUT)
                {
                    logger.Warn($"Wrong destination port {port} for trigger {trigger}");
                    continue;
                }

                portTrigger.DestinationPortRules.Add(new DestinationTriggerRule
                                                     {
                                                         Port = destinationPort,
                                                         Action = destinationPortState.Action,
                                                         DelayAfterAction = destinationPortState.DelayAfterActionInMilliseconds > 0
                                                             ? TimeSpan.FromMilliseconds(destinationPortState.DelayAfterActionInMilliseconds)
                                                             : null,
                                                         DelayBeforeAction = destinationPortState.DelayBeforeActionInMilliseconds > 0
                                                             ? TimeSpan.FromMilliseconds(destinationPortState.DelayBeforeActionInMilliseconds)
                                                             : null
                                                     });
            }

            if (trigger.AdditionalConditions != null)
            {
                var additionalConditions = new AdditionalConditions
                                           {
                                               Type = trigger.AdditionalConditions.Type,
                                               Status = trigger.AdditionalConditions.Status,
                                           };

                foreach (var additionalPort in trigger.AdditionalConditions.Ports)
                {
                    if (!TryGetDevicePort(result, additionalPort.DeviceId, additionalPort.PortId, out var sourcePort))
                    {
                        logger.Warn($"Can't find additional source device/port for trigger {trigger}");
                        continue;
                    }

                    additionalConditions.Ports.Add(sourcePort);
                }

                if (additionalConditions.Ports.Any())
                    portTrigger.AdditionalConditions = additionalConditions;
            }

            if (portTrigger.DestinationPortRules.Any())
                port.TriggerRules.Add(portTrigger);
        }

        return result;
    }

    private static List<Scheduler> GetSchedulers(IReadOnlyDictionary<string, Device> devices, AutomationConfig automationConfig)
    {
        var result = new List<Scheduler>();

        foreach (var schedulerRule in automationConfig.Schedulers)
        {
            var scheduler = new Scheduler
                            {
                                Title = schedulerRule.Title,
                                Type = schedulerRule.Type,
                            };

            if (schedulerRule.Conditions != null)
            {
                var conditions = new SchedulerConditions
                                 {
                                     Type = schedulerRule.Conditions.Type,
                                     Status = schedulerRule.Conditions.Status,
                                     Period = schedulerRule.Conditions.Period,
                                 };

                foreach (var conditionPort in schedulerRule.Conditions.Ports)
                {
                    if (!TryGetDevicePort(devices, conditionPort.DeviceId, conditionPort.PortId, out var port))
                    {
                        logger.Warn($"Can't find source device/port for scheduler {schedulerRule}");
                        continue;
                    }

                    conditions.Ports.Add(port);
                }

                scheduler.Conditions = conditions;
            }

            foreach (var destinationPortState in schedulerRule.DestinationPortStates)
            {
                if (!TryGetDevicePort(devices, destinationPortState.DeviceId, destinationPortState.PortId, out var destinationPort))
                {
                    logger.Warn($"Can't find destination device/port for scheduler {schedulerRule}");
                    continue;
                }

                if (destinationPort.Type != DevicePortType.OUT)
                {
                    logger.Warn($"Wrong destination port {destinationPort} for scheduler {schedulerRule}");
                    continue;
                }

                scheduler.DestinationPortRules.Add(new SchedulerDestinationPortRule
                                                   {
                                                       Port = destinationPort,
                                                       Action = destinationPortState.Action,
                                                   });
            }

            if (scheduler.DestinationPortRules.Any() &&
                (scheduler.Type != SchedulerType.StartByCondition || (scheduler.Conditions?.Ports.Any() ?? false)))
                result.Add(scheduler);
        }

        return result;
    }

    private static List<Location> GetLocations(HomeMapConfig homeMapConfig, IReadOnlyDictionary<string, Device> devices)
    {
        var itemIds = new HashSet<Guid>();

        return homeMapConfig.Locations
                            .Select(x => CreateLocation(x, devices, itemIds))
                            .ToList();
    }

    private static Location CreateLocation(Configurations.Location location,
        IReadOnlyDictionary<string, Device> devices, ISet<Guid> itemIds, Location? parent = null)
    {
        var locationModel = new Location
                            {
                                Name = location.Name,
                                Id = GetItemId(parent?.Id, location.Name, itemIds),
                                Parent = parent,
                            };

        foreach (var locationItem in location.Items)
        {
            if (!devices.TryGetValue(locationItem.DeviceId, out var device) ||
                !device.Ports.TryGetValue(locationItem.PortId, out var port))
                continue;

            locationModel.Items.Add(new LocationItems
                                    {
                                        ActionId = GetItemId(parent?.Id, port.Name, itemIds),
                                        Device = device,
                                        Port = port,
                                        CustomName = locationItem.CustomName,
                                    });
        }

        foreach (var locationItem in location.ItemsConditions)
        {
            var locationCondition = new LocationCondition
                                    {
                                        Status = locationItem.Status,
                                        Type = locationItem.Type,
                                    };

            switch (locationItem.Type)
            {
                case LocationConditionType.PortOutCurrentStatus when locationItem.Status.HasValue:
                {
                    foreach (var device in devices.Values)
                    {
                        foreach (var (_, port) in device.Ports.Where(x => x.Value.Type == DevicePortType.OUT))
                        {
                            locationCondition.Items.Add(new LocationItems
                                                        {
                                                            ActionId = GetItemId(parent?.Id, port.Name, itemIds),
                                                            Device = device,
                                                            Port = port,
                                                        });
                        }
                    }

                    break;
                }
            }

            if (locationCondition.Items.Any())
                locationModel.ItemsConditions.Add(locationCondition);
        }

        foreach (var subLocation in location.SubLocations)
        {
            locationModel.SubLocations.Add(CreateLocation(subLocation, devices, itemIds, locationModel));
        }

        return locationModel;
    }

    private static bool TryGetDevicePort(IReadOnlyDictionary<string, Device> devices, string deviceId, int portId, [MaybeNullWhen(false)] out DevicePort port)
    {
        port = null;
        return devices.TryGetValue(deviceId, out var device) &&
               device.Ports.TryGetValue(portId, out port);
    }

    private static string GetItemId(string? parentId, string itemName, ISet<Guid> itemIds)
    {
        Guid id;
        var counter = 0;

        do
        {
            id = GuidHelper.Create(GuidHelper.IsoOidNamespace, $"{parentId}{itemName}{counter}");
            counter++;

        } while (itemIds.Contains(id));

        itemIds.Add(id);
        return id.ToString();
    }
}