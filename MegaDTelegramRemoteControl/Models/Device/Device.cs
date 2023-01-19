using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record Device(string Name, string Ip, string Pwd)
{
    public Dictionary<string, DevicePort> Ports { get; } = new();
}