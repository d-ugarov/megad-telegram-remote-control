using SmartHome.Common.Interfaces;
using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;
using SmartHome.Device.MegaD.CommandParsers;

namespace SmartHome.Device.MegaD;

internal class MegaDCommandParser : IDeviceCommandParser
{
    private static readonly Dictionary<DevicePortOutMode, IStatusParser> parsersOutPorts;
    private static readonly IStatusParser parserInPorts;
    private static readonly Dictionary<string, DeviceEventType> deviceEventTypes;

    static MegaDCommandParser()
    {
        parsersOutPorts = new()
                          {
                              {DevicePortOutMode.SW, new StatusParserOutSw()}
                          };
        parserInPorts = new StatusParserIn();
        deviceEventTypes = new()
                           {
                               {"st", DeviceEventType.DeviceStarted},
                               {"pt", DeviceEventType.PortEvent},
                           };
    }

    #region Events

    public DeviceEvent ParseEvent(IDevice<IDevicePort> device, DeviceEventRaw eventRaw)
    {
        var result = new DeviceEvent();

        if (!TryGetEventType(eventRaw, out var portId, out var eventType) || portId is null || eventType is null)
            return result;

        result.Type = eventType.Value;

        switch (result.Type)
        {
            case DeviceEventType.PortEvent:
            {
                if (!TryGetDevicePort(portId.Value, device.Ports, out var portEvent) || portEvent is null)
                    return result;

                result.Event = portEvent;

                switch (result.Event!.Port.Type)
                {
                    case DevicePortType.IN:
                    {
                        result.Event.In = GetInPortEvent(eventRaw);
                        break;
                    }
                    case DevicePortType.OUT:
                    {
                        result.Event.Out = GetOutPortEvent(eventRaw);
                        break;
                    }
                }

                break;
            }
        }

        result.IsParsedSuccessfully = true;
        return result;
    }

    private static bool TryGetEventType(DeviceEventRaw eventRaw, out int? portId, out DeviceEventType? eventType)
    {
        foreach (var (key, value) in eventRaw.Items)
        {
            foreach (var (command, type) in deviceEventTypes)
            {
                if (!key.Equals(command) || !int.TryParse(value, out var intValue))
                    continue;

                eventType = type;
                portId = intValue;
                return true;
            }
        }

        eventType = null;
        portId = null;
        return false;
    }

    private static bool TryGetDevicePort(int portId, IReadOnlyDictionary<int, IDevicePort> devicePorts, out DevicePortEvent? portEvent)
    {
        if (!devicePorts.TryGetValue(portId, out var port))
        {
            portEvent = null;
            return false;
        }

        portEvent = new DevicePortEvent {Port = port};
        return true;
    }

    private static DevicePortInEvent GetInPortEvent(DeviceEventRaw eventRaw)
    {
        var inPortEvent = new DevicePortInEvent
                          {
                              Command = DevicePortInCommand.KeyPressed,
                          };

        foreach (var (key, value) in eventRaw.Items)
        {
            switch (key)
            {
                case "m":
                {
                    if (int.TryParse(value, out var type))
                    {
                        inPortEvent.Command = type switch
                        {
                            1 => DevicePortInCommand.KeyReleased,
                            2 => DevicePortInCommand.LongClick,
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
                            1 => DevicePortInCommand.Click,
                            2 => DevicePortInCommand.DoubleClick,
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

    private static DevicePortOutEvent GetOutPortEvent(DeviceEventRaw eventRaw)
    {
        var outPortEvent = new DevicePortOutEvent
                           {
                               Command = DevicePortOutCommand.Unknown,
                           };

        foreach (var (key, value) in eventRaw.Items)
        {
            switch (key)
            {
                case "v":
                {
                    if (int.TryParse(value, out var cmd) &&
                        Enum.IsDefined(typeof(DevicePortOutCommand), cmd))
                    {
                        outPortEvent.Command = (DevicePortOutCommand)cmd;
                    }

                    break;
                }
            }
        }

        return outPortEvent;
    }

    #endregion

    #region Port status

    public DevicePortInfo ParseStatus(IDevicePort devicePort, string portStatusRaw)
    {
        var parser = devicePort.Type switch
        {
            DevicePortType.IN => parserInPorts,
            DevicePortType.OUT when parsersOutPorts.TryGetValue(devicePort.OutMode!.Value, out var outParser) => outParser,
            _ => throw new Exception($"Port type {devicePort.OutMode} not supported")
        };

        return parser.Parse(devicePort, portStatusRaw);
    }

    #endregion
}