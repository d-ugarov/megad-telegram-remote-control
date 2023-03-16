using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using DevicePortType = MegaDTelegramRemoteControl.Infrastructure.Configurations.DevicePortType;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePort : IDevicePort
{
    private readonly DeviceOutPortMode? outMode;
    private List<Device>? triggersRequiredDevices;

    public required int Id { get; init; }
    public required DevicePortType Type { get; init; }
    public required string Name { get; init; }

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

    public List<Device> TriggerRulesRequiredDevices
    {
        get
        {
            if (triggersRequiredDevices != null)
                return triggersRequiredDevices;

            triggersRequiredDevices = TriggerRules.Where(x => x.AdditionalConditions != null)
                                                  .SelectMany(x => x.AdditionalConditions!.Ports.Select(p => p.Device))
                                                  .Union(TriggerRules.SelectMany(t => t.DestinationPortRules.Select(p => p.Port.Device)))
                                                  .Distinct()
                                                  .ToList();
            return triggersRequiredDevices;
        }
    }

    public override string ToString() => $"port: {Name} " +
                                         $"({Type},{(Type == DevicePortType.OUT ? $" {outMode}," : "")} {Device.Name})";

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)Type, Name, Device.GetHashCode());
    }
}