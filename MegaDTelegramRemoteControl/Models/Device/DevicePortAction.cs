using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortAction
    {
        public DeviceOutPortCommand? SWCommand { get; set; }

        public static DevicePortAction DefaultSWSwitch => new() {SWCommand = DeviceOutPortCommand.Switch};
    }
}