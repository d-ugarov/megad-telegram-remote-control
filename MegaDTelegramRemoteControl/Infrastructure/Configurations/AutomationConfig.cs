using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record AutomationConfig
{
    public List<TriggerRule> Triggers { get; init; } = new();
    public List<SchedulerRule> Schedulers { get; init; } = new();
}

#region Scheduler

public record SchedulerRule
{
    public string Title { get; init; } = "";
    public SchedulerType Type { get; init; }
    public SchedulerRuleConditions? Conditions { get; init; }
    public List<TriggerRuleDestinationPortState> DestinationPortStates { get; init; } = null!;
}

public enum SchedulerType
{
    StartOnTime,
    StartByCondition,
}

public record SchedulerRuleConditions : TriggerRuleAdditionalConditions
{
    public TimeSpan Period { get; init; }
}

#endregion

#region Triggers

public record TriggerRule
{
    public TriggerRuleSourcePortState SourcePortState { get; init; } = null!;
    public TriggerRuleAdditionalConditions? AdditionalConditions { get; init; }
    public List<TriggerRuleDestinationPortState> DestinationPortStates { get; init; } = null!;
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
    public List<TriggerRulePort> Ports { get; init; } = new();
    public ConditionType Type { get; init; }
    public InOutSWStatus? Status { get; init; }
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
    public List<DeviceInPortCommand> InPortCommands { get; init; } = new();
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

#endregion