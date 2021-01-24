using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Device
    {
        public string Name { get; init; } = null!;
        public string Ip { get; init; } = null!;
        public string Pwd { get; init; } = null!;

        public Dictionary<string, DevicePort> Ports { get; init; } = new();
    }
}