using SmartHome.Common.Models.Device;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record Device(string Id, string Name, string Ip, string Pwd) : IDevice<DevicePort>
{
    public Dictionary<int, DevicePort> Ports { get; } = new();

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Ip, Pwd);
    }
}