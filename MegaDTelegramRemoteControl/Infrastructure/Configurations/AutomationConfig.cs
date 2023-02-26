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
    public TriggerRuleAdditionalConditions? AdditionalConditions { get; set; }
    public TriggerRuleDestinationPortState[] DestinationPortStates { get; set; } = null!;
    public TriggerResult Result { get; init; }
    public bool IsFinal { get; init; }
}

public record TriggerRulePort
{
    public string DeviceId { get; init; } = null!;
    public int PortId { get; init; }
}

public record TriggerRuleSourcePortState : TriggerRulePort
{
    public TriggerRuleSourcePortStatus Status { get; init; } = null!;
}

public record TriggerRuleAdditionalConditions
{
    public TriggerRulePort[] Ports { get; set; } = Array.Empty<TriggerRulePort>();
    public ConditionType Type { get; set; }
    public InOutSWStatus? Status { get; set; }
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
    public int PortId { get; init; }
    public TriggerRuleAction Action { get; init; } = null!;
    public int DelayBeforeActionInMilliseconds { get; init; }
    public int DelayAfterActionInMilliseconds { get; init; }
}

public record TriggerRuleSourcePortStatus
{
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