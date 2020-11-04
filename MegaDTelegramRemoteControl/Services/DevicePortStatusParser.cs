using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services
{
    public class DevicePortStatusParser : IDevicePortStatusParser
    {
        private interface IStatusParser
        {
            DevicePortStatus Parse(DevicePort devicePort, string portStatus);
        }

        private class SwStatusParser : IStatusParser
        {
            public DevicePortStatus Parse(DevicePort devicePort, string portStatus)
            {
                return new DevicePortStatus
                       {
                           SWStatus = Enum.Parse<SWStatus>(portStatus, true),
                           Port = devicePort,
                       };
            }
        }

        private static readonly Dictionary<DeviceOutPortMode, IStatusParser> parsers;

        static DevicePortStatusParser()
        {
            parsers = new Dictionary<DeviceOutPortMode, IStatusParser>
                      {
                          {DeviceOutPortMode.SW, new SwStatusParser()}
                      };
        }
        
        public DevicePortStatus ParseStatus(DevicePort port, string portStatus)
        {
            if (!parsers.TryGetValue(port.OutMode ?? default, out var parser))
                throw new Exception($"Port type {port.OutMode} not supported");
            
            return parser.Parse(port, portStatus);
        }
    }
}