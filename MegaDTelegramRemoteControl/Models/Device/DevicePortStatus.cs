using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DevicePortStatus
{
    public DevicePort Port { get; init; } = null!;
    public SWStatus SWStatus { get; init; }

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