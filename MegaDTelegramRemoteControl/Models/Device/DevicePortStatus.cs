using MegaDTelegramRemoteControl.Infrastructure.Configurations;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortStatus
    {
        public DevicePort Port { get; set; }
        public SWStatus SWStatus { get; set; }

        public override string ToString()
        {
            switch (Port.OutMode)
            {
                case DeviceOutPortMode.SW:
                {
                    if (Port.OutModeIcons != null &&
                        Port.OutModeIcons.TryGetValue(SWStatus.ToString(), out var icon))
                        return icon;

                    return $"[{SWStatus}]";
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