using MegaDTelegramRemoteControl.Infrastructure.Configurations;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortStatus
    {
        public DevicePort Port { get; set; } = null!;
        public SWStatus SWStatus { get; set; }

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

    public enum SWStatus
    {
        On, 
        Off,
    }
}