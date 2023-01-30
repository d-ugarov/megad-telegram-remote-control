using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePort
{
    private readonly DeviceOutPortMode? outMode;

    public required string Id { get; init; }
    public required DevicePortType Type { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }

    public required DeviceOutPortMode? OutMode
    {
        get => Type == DevicePortType.OUT
            ? outMode
            : throw new OperationException($"'Out' port data is not available for port type {Type}");
        init => outMode = value;
    }

    public required Dictionary<string, string> InOutSWModeIcons { get; init; }

    public required Device Device { get; init; }

    public List<DevicePortTriggerRule> TriggerRules { get; } = new();

    public override string ToString() => $"port: {Description ?? Name} " +
                                         $"({Type},{(Type == DevicePortType.OUT ? $" {outMode}," : "")} {Device.Name})";
}