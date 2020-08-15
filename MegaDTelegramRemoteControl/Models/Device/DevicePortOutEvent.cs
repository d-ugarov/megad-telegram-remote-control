using MegaDTelegramRemoteControl.Infrastructure.Helpers;

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