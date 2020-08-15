using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services
{
    public class DeviceEventParser : IDeviceEventParser
    {
        private readonly HomeConfig homeConfig;
        
        public DeviceEventParser(HomeConfig homeConfig)
        {
            this.homeConfig = homeConfig;
        }

        public DeviceEvent ParseEvent(string deviceId, List<(string key, string value)> query)
        {
            if (!homeConfig.Devices.TryGetValue(deviceId, out var device))
                return DeviceEvent.UnknownEvent;

            var result = new DeviceEvent
                         {
                             Date = DateTime.UtcNow,
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

                    switch (result.Port.Port.Type)
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
            
            result.Port = new DevicePortEvent {Port = port};
            return true;
        }

        private static void ParseInPort(IEnumerable<(string key, string value)> query, DeviceEvent result)
        {
            result.Port.In = new DevicePortInEvent
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
                            result.Port.In.Command = type switch
                            {
                                1 => DeviceInPortCommand.KeyReleased,
                                2 => DeviceInPortCommand.LongClick,
                                _ => result.Port.In.Command
                            };
                        }
                        break;
                    }
                    case "click":
                    {
                        if (int.TryParse(value, out var type))
                        {
                            result.Port.In.Command = type switch
                            {
                                1 => DeviceInPortCommand.Click,
                                2 => DeviceInPortCommand.DoubleClick,
                                _ => result.Port.In.Command
                            };
                        }
                        break;
                    }
                    case "cnt":
                    {
                        if (int.TryParse(value, out var counter))
                            result.Port.In.Counter = counter;
                        break;
                    }
                }
            }
        }

        private static void ParseOutPort(IEnumerable<(string key, string value)> query, DeviceEvent result)
        {
            result.Port.Out = new DevicePortOutEvent
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
                            result.Port.Out.Command = (DeviceOutPortCommand)cmd;
                        }
                        break;
                    }
                }
            }
        }
    }
}