using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePort
    {
        public string Id { get; set; } = null!;
        public DevicePortType Type { get; set; }
        public string Name { get; set; } = null!;
        public DeviceOutPortMode? OutMode { get; set; }
        public Dictionary<string, string> OutModeIcons { get; set; } = new();

        public Device Device { get; set; } = null!;
    }
}