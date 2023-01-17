using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;
using LocationItems = MegaDTelegramRemoteControl.Models.Device.LocationItems;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers
{
    public static class ConfigHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        public static HomeConfig MakeConfig(DevicesConfig devicesConfig, 
            HomeMapConfig homeMapConfig,
            AutomationConfig automationConfig)
        {
            var devices = GetDevices(devicesConfig, automationConfig);
            var locations = GetLocations(homeMapConfig, devices);

            return new HomeConfig
                   {
                       Devices = devices,
                       Locations = locations
                   };
        }

        private static Dictionary<string, Device> GetDevices(DevicesConfig devicesConfig, AutomationConfig automationConfig)
        {
            var result = new Dictionary<string, Device>();
            
            bool TryGetPort(string deviceId, string portId, out DevicePort? port)
            {
                port = null;
                return result.TryGetValue(deviceId, out var device) &&
                       device.Ports.TryGetValue(portId, out port);
            }
            
            foreach (var device in devicesConfig.Devices)
            {
                var deviceModel = new Device
                                  {
                                      Ip = device.Ip,
                                      Name = device.Name,
                                      Pwd = device.Pwd,
                                  };

                foreach (var port in device.DevicePorts)
                {
                    deviceModel.Ports[port.Id] = new DevicePort
                                                 {
                                                     Id = port.Id,
                                                     Name = port.Name,
                                                     Description = port.Description,
                                                     Type = port.Type,
                                                     OutMode = port.OutMode,
                                                     OutModeIcons = port.OutModeIcons,
                                                     Device = deviceModel,
                                                 };
                }

                result[device.Id] = deviceModel;
            }

            foreach (var trigger in automationConfig.Triggers)
            {
                if (!TryGetPort(trigger.SourceDeviceId, trigger.SourcePortId, out var port))
                {
                    logger.Warn($"Can't find device/port for trigger {trigger}");
                    continue;
                }

                var portTrigger = new DevicePortTriggerRule
                                  {
                                      Action = trigger.Action,
                                      Mode = trigger.Mode,
                                      SourcePortStatus = trigger.SourcePortStatus,
                                  };

                foreach (var condition in trigger.AdditionalConditions)
                {
                    if (!TryGetPort(condition.SourceDeviceId, condition.SourcePortId, out var conditionPort))
                    {
                        logger.Warn($"Can't find device/port for trigger {trigger}");
                        continue;
                    }

                    portTrigger.AdditionalConditions.Add(new AdditionalDevicePortTriggerRule
                                                         {
                                                             DevicePort = conditionPort!,
                                                             SourcePortStatus = condition.SourcePortStatus,
                                                         });
                }

                port!.TriggerRules.Add(portTrigger);
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
                                            Id = GetItemId(parent?.Id, port.Name, itemIds),
                                            Device = device,
                                            Port = port,
                                            CustomName = locationItem.CustomName,
                                        });
            }

            foreach (var subLocation in location.SubLocations)
            {
                locationModel.SubLocations.Add(CreateLocation(subLocation, devices, itemIds, locationModel));
            }

            return locationModel;
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
}