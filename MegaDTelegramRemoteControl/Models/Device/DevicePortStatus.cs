using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DevicePortStatus
{
    public required DevicePort Port { get; init; }
    public InOutSWStatus InOutSwStatus { get; init; }
    public int InCounter { get; init; }

    public override string ToString()
    {
        switch (Port.Type)
        {
            case DevicePortType.IN:
            {
                return Port.InOutSWModeIcons.TryGetValue(InOutSwStatus.ToString(), out var icon)
                    ? icon
                    : $"[{InOutSwStatus}]";
            }
            case DevicePortType.OUT:
            {
                switch (Port.OutMode)
                {
                    case DeviceOutPortMode.SW:
                    {
                        return Port.InOutSWModeIcons.TryGetValue(InOutSwStatus.ToString(), out var icon)
                            ? icon
                            : $"[{InOutSwStatus}]";
                    }
                }
                break;
            }
        }

        return string.Empty;
    }
}