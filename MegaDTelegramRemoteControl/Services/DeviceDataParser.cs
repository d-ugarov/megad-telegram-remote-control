using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services
{
    public class DeviceDataParser : IDeviceDataParser
    {
        #region Port status parsers
        
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
        
        #endregion

        #region Fields

        private readonly HomeConfig homeConfig;
        
        private static readonly Dictionary<DeviceOutPortMode, IStatusParser> parsers;

        #endregion

        #region Ctors
        
        public DeviceDataParser(HomeConfig homeConfig)
        {
            this.homeConfig = homeConfig;
        }
        
        static DeviceDataParser()
        {
            parsers = new Dictionary<DeviceOutPortMode, IStatusParser>
                      {
                          {DeviceOutPortMode.SW, new SwStatusParser()}
                      };
        }
        
        #endregion

        #region Events
        
        public DeviceEvent ParseEvent(string deviceId, List<(string key, string value)> query)
        {
            if (!homeConfig.Devices.TryGetValue(deviceId, out var device))
                return DeviceEvent.UnknownEvent;

            var result = new DeviceEvent
                         {
                             Device = device,
                         };

            if (!TryParseEventType(query, result, out var portId))
                return result;

            switch (result.Type)
            {
                case DeviceEventType.PortEvent:
                {
                    if (!TryGetDevicePort(portId, result))
                        return result;

                    switch (result.Event.Port.Type)
                    {
                        case DevicePortType.IN:
                        {
                            ParseInPort(query, result);
                            break;
                        }
                        case DevicePortType.OUT:
                        {
                            ParseOutPort(query, result);
                            break;
                        }
                    }
                    
                    break;
                }
            }

            result.IsParsedSuccessfully = true;
            return result;
        }
        
        private static bool TryParseEventType(IEnumerable<(string key, string value)> query, DeviceEvent result, out string portId)
        {
            foreach (var (key, value) in query)
            {
                foreach (var (command, eventType) in Constants.DeviceEventTypes)
                {
                    if (!key.Equals(command)) 
                        continue;
                    
                    result.Type =  eventType;
                    portId = value;
                    return true;
                }
            }

            portId = null;
            return false;
        }
        
        private static bool TryGetDevicePort(string portId, DeviceEvent result)
        {
            if (!result.Device.Ports.TryGetValue(portId, out var port)) 
                return false;

            result.Event = new DevicePortEvent {Port = port, Date = DateTime.UtcNow};
            return true;
        }

        private static void ParseInPort(IEnumerable<(string key, string value)> query, DeviceEvent result)
        {
            result.Event.In = new DevicePortInEvent
                             {
                                 Command = DeviceInPortCommand.KeyPressed,
                             };
            
            foreach (var (key, value) in query)
            {
                switch (key)
                {
                    case "m":
                    {
                        if (int.TryParse(value, out var type))
                        {
                            result.Event.In.Command = type switch
                            {
                                1 => DeviceInPortCommand.KeyReleased,
                                2 => DeviceInPortCommand.LongClick,
                                _ => result.Event.In.Command
                            };
                        }
                        break;
                    }
                    case "click":
                    {
                        if (int.TryParse(value, out var type))
                        {
                            result.Event.In.Command = type switch
                            {
                                1 => DeviceInPortCommand.Click,
                                2 => DeviceInPortCommand.DoubleClick,
                                _ => result.Event.In.Command
                            };
                        }
                        break;
                    }
                    case "cnt":
                    {
                        if (int.TryParse(value, out var counter))
                            result.Event.In.Counter = counter;
                        break;
                    }
                }
            }
        }

        private static void ParseOutPort(IEnumerable<(string key, string value)> query, DeviceEvent result)
        {
            result.Event.Out = new DevicePortOutEvent
                              {
                                  Command = DeviceOutPortCommand.Unknown,
                              };
            
            foreach (var (key, value) in query)
            {
                switch (key)
                {
                    case "v":
                    {
                        if (int.TryParse(value, out var cmd) &&
                            Enum.IsDefined(typeof(DeviceOutPortCommand), cmd))
                        {
                            result.Event.Out.Command = (DeviceOutPortCommand)cmd;
                        }
                        break;
                    }
                }
            }
        }
        
        #endregion

        #region Port status

        public DevicePortStatus ParseStatus(DevicePort port, string portStatus)
        {
            if (!parsers.TryGetValue(port.OutMode ?? default, out var parser))
                throw new Exception($"Port type {port.OutMode} not supported");
            
            return parser.Parse(port, portStatus);
        }

        #endregion
    }
}