using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record AutomationConfig
{
    public TriggerRule[] Triggers { get; init; } = Array.Empty<TriggerRule>();
}

public record TriggerRule
{
    public TriggerRuleSourcePortState SourcePortState { get; set; } = null!;
    public TriggerRuleAdditionalConditions? AdditionalConditions { get; set; }
    public TriggerRuleDestinationPortState[] DestinationPortStates { get; set; } = null!;
    public TriggerResult Result { get; init; }
}

public record TriggerRulePort
{
    public string DeviceId { get; init; } = null!;
    public string PortId { get; init; } = null!;
}

public record TriggerRuleSourcePortState : TriggerRulePort
{
    public TriggerRuleSourcePortStatus Status { get; init; } = null!;
}

public record TriggerRuleAdditionalConditions
{
    public TriggerRulePort[] Ports { get; set; } = Array.Empty<TriggerRulePort>();
    public ConditionType Type { get; set; }
    public SWStatus? Status { get; set; }
}

public enum ConditionType
{
    StatesAreEqual,
    StatesAnyEqual,
    StatesNotEqual,
}

public record TriggerRuleDestinationPortState
{
    public string DeviceId { get; init; } = null!;
    public string PortId { get; init; } = null!;
    public TriggerRuleAction Action { get; init; } = null!;
}

public record TriggerRuleSourcePortStatus
{
    public SWStatus[] SWStatuses { get; init; } = Array.Empty<SWStatus>();
    public DeviceInPortCommand[] InPortCommands { get; init; } = Array.Empty<DeviceInPortCommand>();
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