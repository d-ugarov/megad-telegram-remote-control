using MegaDTelegramRemoteControl.Infrastructure.Configurations;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DevicePortInfo
{
    public required DevicePort Port { get; init; }
    public required DevicePortStatus Status { get; init; }

    public override string ToString()
    {
        switch (Port.Type)
        {
            case DevicePortType.IN:
            {
                return Port.InOutSWModeIcons.TryGetValue(Status.InOutSwStatus.ToString(), out var icon)
                    ? icon
                    : $"[{Status.InOutSwStatus}]";
            }
            case DevicePortType.OUT:
            {
                switch (Port.OutMode)
                {
                    case DeviceOutPortMode.SW:
                    {
                        return Port.InOutSWModeIcons.TryGetValue(Status.InOutSwStatus.ToString(), out var icon)
                            ? icon
                            : $"[{Status.InOutSwStatus}]";
                    }
                }
                break;
            }
        }

        return string.Empty;
    }
}