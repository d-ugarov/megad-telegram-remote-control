using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DeviceEvent
{
    private DevicePortEvent? port;

    public DeviceEventType Type { get; set; }

    public bool IsParsedSuccessfully { get; set; }

    public Device? Device { get; set; }

    public DevicePortEvent? Event
    {
        get => Type == DeviceEventType.PortEvent
            ? port
            : throw new OperationException($"Port is not available for event type {Type}");
        set => port = value;
    }

    public IReadOnlyCollection<NewEventData> EventData { get; }

    public DeviceEvent(IReadOnlyCollection<NewEventData> eventData)
    {
        EventData = eventData;
    }

    public override string ToString()
    {
        var str = new StringBuilder($"Event: {Type}");

        str.Append(IsParsedSuccessfully ? " (parsed)" : " (unknown)");

        if (Device != null)
            str.Append($", device: {Device.Name}");

        if (port?.Port != null)
            str.Append($", {port}");

        if (!IsParsedSuccessfully && EventData.Any())
            str.Append($", event data: {string.Join(", ", EventData.Select(x => $"{x.Key}={x.Value}"))}");

        return str.ToString();
    }
}