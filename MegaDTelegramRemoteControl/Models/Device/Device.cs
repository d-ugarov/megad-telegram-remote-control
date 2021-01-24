using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Device
    {
        public string Name { get; set; } = null!;
        public string Ip { get; set; } = null!;
        public string Pwd { get; set; } = null!;

        public Dictionary<string, DevicePort> Ports { get; set; } = new();
    }
}