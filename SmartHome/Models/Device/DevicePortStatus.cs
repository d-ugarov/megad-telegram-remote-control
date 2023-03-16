using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public record struct DevicePortStatus
{
    public InOutSWStatus InOutSwStatus { get; init; }
    public int InCounter { get; init; }
}