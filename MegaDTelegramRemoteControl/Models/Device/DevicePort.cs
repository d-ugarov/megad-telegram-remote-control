using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public class DevicePort
{
    private readonly DeviceOutPortMode? outMode;

    public string Id { get; init; } = null!;
    public DevicePortType Type { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    public DeviceOutPortMode? OutMode
    {
        get => Type == DevicePortType.OUT
            ? outMode
            : throw new OperationException($"'Out' port data is not available for port type {Type}");
        init => outMode = value;
    }

    public Dictionary<string, string> OutModeIcons { get; init; } = new();

    public Device Device { get; init; } = null!;

    public List<DevicePortTriggerRule> TriggerRules { get; init; } = new();
}