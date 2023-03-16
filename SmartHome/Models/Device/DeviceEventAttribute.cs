using System;

namespace MegaDTelegramRemoteControl.Models.Device;

[AttributeUsage(AttributeTargets.Field)]
public class DeviceEventAttribute : Attribute
{
    public string Command { get; }

    public DeviceEventAttribute(string command)
    {
        Command = command;
    }
}