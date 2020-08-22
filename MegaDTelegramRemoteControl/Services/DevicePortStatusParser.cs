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
            DevicePortStatus Parse(string data);
        }

        private class SwStatusParser : IStatusParser
        {
            public DevicePortStatus Parse(string data)
            {
                return new DevicePortStatus {SWStatus = Enum.Parse<SWStatus>(data, true)};
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
        
        public DevicePortStatus ParseStatus(DevicePort port, string data)
        {
            if (!parsers.TryGetValue(port.OutMode ?? default, out var parser))
                throw new Exception($"Port type {port.OutMode} not supported");

            var result = parser.Parse(data);

            result.Port = port;

            return result;
        }
    }
}