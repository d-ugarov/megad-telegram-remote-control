using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record DevicesConfig
{
    public List<Device> Devices { get; init; } = new();
}

/// <summary> MegaD </summary>
public record Device
{
    /// <summary>
    /// Device id
    /// <para>Used to distinguish events from multiple MegaDs</para>
    /// </summary>
    public string Id { get; init; } = null!;

    public string Name { get; init; } = null!;
    public string Ip { get; init; } = null!;
    public string Pwd { get; init; } = null!;

    public List<DevicePort> DevicePorts { get; init; } = new();
}

public record DevicePort
{
    public int Id { get; init; }
    public DevicePortType Type { get; init; }
    public string Name { get; init; } = null!;
    public DeviceOutPortMode? OutMode { get; init; }
    public Dictionary<string, string> InOutSWModeIcons { get; init; } = new();
}

public enum DevicePortType
{
    IN,
    OUT,
    DSen,
    I2C,
    ADC,
}

public enum DeviceInPortMode
{
    P,
    PR,
    R,
    C,
}

public enum DeviceOutPortMode
{
    SW,
    PWM,
    SWLink,
    DS2413,
}