using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using System;
using System.Collections.Generic;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;
using LocationItems = MegaDTelegramRemoteControl.Models.Device.LocationItems;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers
{
    public static class ConfigHelper
    {
        public static HomeConfig MakeConfig(DevicesConfig devicesConfig, HomeMapConfig homeMapConfig)
        {
            var result = new HomeConfig
                         {
                             Devices = new Dictionary<string, Device>(),
                             Locations = new List<Location>(),
                         };

            FillDevices(devicesConfig, result);
            FillLocations(homeMapConfig, result);

            return result;
        }

        private static void FillDevices(DevicesConfig devicesConfig, HomeConfig result)
        {
            if (devicesConfig?.Devices == null) 
                return;
            
            foreach (var device in devicesConfig.Devices)
            {
                var deviceModel = new Device
                                  {
                                      Ip = device.Ip,
                                      Name = device.Name,
                                      Ports = new Dictionary<string, DevicePort>()
                                  };

                if (device.DevicePorts != null)
                {
                    foreach (var port in device.DevicePorts)
                    {
                        deviceModel.Ports[port.Id] = new DevicePort
                                                     {
                                                         Id = port.Id,
                                                         Name = port.Name,
                                                         Type = port.Type,
                                                         OutMode = port.OutMode,
                                                         OutModeIcons = port.OutModeIcons,
                                                         Device = deviceModel,
                                                     };
                    }
                }

                result.Devices[device.Id] = deviceModel;
            }
        }

        private static void FillLocations(HomeMapConfig homeMapConfig, HomeConfig result)
        {
            if (homeMapConfig?.Locations == null) 
                return;
            
            var itemIds = new HashSet<Guid>();
            
            foreach (var location in homeMapConfig.Locations)
            {
                result.Locations.Add(CreateLocation(location, result.Devices, itemIds));
            }
        }

        private static Location CreateLocation(Configurations.Location location, IReadOnlyDictionary<string, Device> devices, 
            ISet<Guid> itemIds, Location parent = null)
        {
            var locationModel = new Location
                                {
                                    Name = location.Name,
                                    Id = GetItemId(parent?.Id, location.Name, itemIds),
                                    Items = new List<LocationItems>(),
                                    SubLocations = new List<Location>(),
                                    Parent = parent,
                                };

            if (location.Items != null)
            {
                foreach (var locationItem in location.Items)
                {
                    if (devices.TryGetValue(locationItem.DeviceId, out var device) &&
                        device.Ports.TryGetValue(locationItem.PortId, out var port))
                    {
                        locationModel.Items.Add(new LocationItems
                                                {
                                                    Id = GetItemId(parent?.Id, port.Name, itemIds),
                                                    Device = device,
                                                    Port = port,
                                                });
                    }
                }
            }

            if (location.SubLocations != null)
            {
                foreach (var subLocation in location.SubLocations)
                {
                    locationModel.SubLocations.Add(CreateLocation(subLocation, devices, itemIds, locationModel));
                }
            }

            return locationModel;
        }

        private static string GetItemId(string parentId, string itemName, ISet<Guid> itemIds)
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