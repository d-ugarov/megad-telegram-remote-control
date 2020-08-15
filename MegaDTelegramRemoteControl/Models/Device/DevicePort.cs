using MegaDTelegramRemoteControl.Infrastructure.Configurations;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePort
    {
        public string Id { get; set; }
        public DevicePortType Type { get; set; }
        public string Name { get; set; }
        public DeviceOutPortMode? OutMode { get; set; }
        
        public Device Device { get; set; }
    }
}