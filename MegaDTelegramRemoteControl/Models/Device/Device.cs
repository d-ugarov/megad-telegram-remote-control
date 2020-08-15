using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Device
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Pwd { get; set; }
        
        public Dictionary<string, DevicePort> Ports { get; set; }
    }
}