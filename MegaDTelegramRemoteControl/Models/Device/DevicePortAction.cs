using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortAction
    {
        public DeviceOutPortCommand Command { get; set; }

        public static DevicePortAction DefaultSwitch => new() {Command = DeviceOutPortCommand.Switch};
    }
}