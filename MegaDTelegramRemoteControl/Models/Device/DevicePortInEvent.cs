using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortInEvent
    {
        public DeviceInPortCommand Command { get; set; }
        
        public int Counter { get; set; }

        public override string ToString()
        {
            return $"command: {Command.GetDescription()}, counter: {Counter}";
        }
    }
}