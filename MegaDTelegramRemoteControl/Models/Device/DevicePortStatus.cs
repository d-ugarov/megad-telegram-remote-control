using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DevicePortStatus
{
    public required DevicePort Port { get; init; }
    public required SWStatus SWStatus { get; init; }

    public override string ToString()
    {
        switch (Port.OutMode)
        {
            case DeviceOutPortMode.SW:
            {
                return Port.OutModeIcons.TryGetValue(SWStatus.ToString(), out var icon)
                    ? icon
                    : $"[{SWStatus}]";
            }
        }

        return "";
    }
}