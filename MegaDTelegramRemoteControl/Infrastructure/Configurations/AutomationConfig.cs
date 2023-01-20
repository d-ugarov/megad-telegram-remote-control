using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record AutomationConfig
{
    public List<TriggerRule> Triggers { get; init; } = new();
}

public record TriggerRule
{
    public TriggerRuleSourcePortState SourcePortState { get; set; } = null!;
    public List<TriggerRuleDestinationPortState> DestinationPortStates { get; set; } = null!;
    public TriggerResult Result { get; init; }
}

public record TriggerRuleSourcePortState
{
    public string DeviceId { get; init; } = null!;
    public string PortId { get; init; } = null!;
    public TriggerRuleSourcePortStatus Status { get; init; } = null!;
}

public record TriggerRuleDestinationPortState
{
    public string DeviceId { get; init; } = null!;
    public string PortId { get; init; } = null!;
    public TriggerRuleAction Action { get; init; } = null!;
}

public record TriggerRuleSourcePortStatus
{
    public IEnumerable<SWStatus> SWStatuses { get; init; } = Array.Empty<SWStatus>();
    public IEnumerable<DeviceInPortCommand> InPortCommands { get; init; } = Array.Empty<DeviceInPortCommand>();
}

public record TriggerRuleAction
{
    public DeviceOutPortCommand? SWCommand { get; init; }
}

public enum TriggerResult
{
    Default,
    DoNothing,
}