using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Models.Device.Enums;

namespace SmartHome.Common.Models.Device;

public record DeviceEvent
{
    private DevicePortEvent? portEvent;

    public DeviceEventType Type { get; set; }
    public bool IsParsedSuccessfully { get; set; }

    public DevicePortEvent? Event
    {
        get => Type == DeviceEventType.PortEvent
            ? portEvent
            : throw new OperationException($"Port event is not available for event type {Type}");
        set => portEvent = value;
    }
}

public record DevicePortEvent
{
    private DevicePortInEvent? inPortData;
    private DevicePortOutEvent? outPortData;

    public required IDevicePort Port { get; init; }

    public DevicePortInEvent? In
    {
        get => Port.Type == DevicePortType.IN
            ? inPortData
            : throw new OperationException($"'IN' port event is not available for port type {Port.Type}");
        set => inPortData = value;
    }

    public DevicePortOutEvent? Out
    {
        get => Port.Type == DevicePortType.OUT
            ? outPortData
            : throw new OperationException($"'Out' port event is not available for port type {Port.Type}");
        set => outPortData = value;
    }
}

public record DevicePortInEvent
{
    public DevicePortInCommand Command { get; set; }
    public int Counter { get; set; }
}

public record DevicePortOutEvent
{
    public DevicePortOutCommand Command { get; set; }
}