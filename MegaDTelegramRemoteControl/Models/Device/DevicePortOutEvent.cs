using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortOutEvent
    {
        public DeviceOutPortCommand Command { get; set; }

        public override string ToString()
        {
            return $"command: {Command.GetDescription()}";
        }
    }
}