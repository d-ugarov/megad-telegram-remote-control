using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Services.Interfaces;
using MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;
using System;
using System.Collections.Generic;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services.MegaDServices;

public class MegaDCommandParser : IDeviceCommandParser
{
    private readonly IHomeService homeService;
    private static readonly Dictionary<DeviceOutPortMode, IStatusParser> parsers;

    public MegaDCommandParser(IHomeService homeService)
    {
        this.homeService = homeService;
    }

    static MegaDCommandParser()
    {
        parsers = new Dictionary<DeviceOutPortMode, IStatusParser>
                  {
                      {DeviceOutPortMode.SW, new SwStatusParser()}
                  };
    }

    #region Events

    public DeviceEvent ParseEvent(string deviceId, IReadOnlyCollection<NewEventData> eventData)
    {
        var result = new DeviceEvent(eventData);

        if (!homeService.Devices.TryGetValue(deviceId, out var device))
            return result;

        result.Device = device;

        if (!TryGetEventType(eventData, out var portId, out var eventType) || portId is null || eventType is null)
            return result;

        result.Type = eventType.Value;

        switch (result.Type)
        {
            case DeviceEventType.PortEvent:
            {
                if (!TryGetDevicePort(portId, result.Device.Ports, out var portEvent) || portEvent is null)
                    return result;

                result.Event = portEvent;

                switch (result.Event!.Port.Type)
                {
                    case DevicePortType.IN:
                    {
                        result.Event.In = GetInPortEvent(eventData);
                        break;
                    }
                    case DevicePortType.OUT:
                    {
                        result.Event.Out = GetOutPortEvent(eventData);
                        break;
                    }
                }

                break;
            }
        }

        result.IsParsedSuccessfully = true;
        return result;
    }

    private static bool TryGetEventType(IEnumerable<NewEventData> eventData, out string? portId, out DeviceEventType? eventType)
    {
        foreach (var (key, value) in eventData)
        {
            foreach (var (command, type) in Constants.DeviceEventTypes)
            {
                if (!key.Equals(command))
                    continue;

                eventType = type;
                portId = value;
                return true;
            }
        }

        eventType = null;
        portId = null;
        return false;
    }

    private static bool TryGetDevicePort(string portId, IReadOnlyDictionary<string, DevicePort> devicePorts, out DevicePortEvent? portEvent)
    {
        if (!devicePorts.TryGetValue(portId, out var port))
        {
            portEvent = null;
            return false;
        }

        portEvent = new DevicePortEvent {Port = port, Date = DateTime.UtcNow};
        return true;
    }

    private static DevicePortInEvent GetInPortEvent(IEnumerable<NewEventData> eventData)
    {
        var inPortEvent = new DevicePortInEvent
                          {
                              Command = DeviceInPortCommand.KeyPressed,
                          };

        foreach (var (key, value) in eventData)
        {
            switch (key)
            {
                case "m":
                {
                    if (int.TryParse(value, out var type))
                    {
                        inPortEvent.Command = type switch
                        {
                            1 => DeviceInPortCommand.KeyReleased,
                            2 => DeviceInPortCommand.LongClick,
                            _ => inPortEvent.Command,
                        };
                    }

                    break;
                }
                case "click":
                {
                    if (int.TryParse(value, out var type))
                    {
                        inPortEvent.Command = type switch
                        {
                            1 => DeviceInPortCommand.Click,
                            2 => DeviceInPortCommand.DoubleClick,
                            _ => inPortEvent.Command,
                        };
                    }

                    break;
                }
                case "cnt":
                {
                    if (int.TryParse(value, out var counter))
                        inPortEvent.Counter = counter;
                    break;
                }
            }
        }

        return inPortEvent;
    }

    private static DevicePortOutEvent GetOutPortEvent(IEnumerable<NewEventData> eventData)
    {
        var outPortEvent = new DevicePortOutEvent
                           {
                               Command = DeviceOutPortCommand.Unknown,
                           };

        foreach (var (key, value) in eventData)
        {
            switch (key)
            {
                case "v":
                {
                    if (int.TryParse(value, out var cmd) &&
                        Enum.IsDefined(typeof(DeviceOutPortCommand), cmd))
                    {
                        outPortEvent.Command = (DeviceOutPortCommand)cmd;
                    }

                    break;
                }
            }
        }

        return outPortEvent;
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