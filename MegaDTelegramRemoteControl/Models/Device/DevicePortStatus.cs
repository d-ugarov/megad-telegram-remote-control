using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System;

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
                    return SWStatus.ToString();
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