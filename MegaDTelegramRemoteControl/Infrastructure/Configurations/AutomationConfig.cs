using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record AutomationConfig
{
    public List<TriggerRule> Triggers { get; init; } = new();
}

public record TriggerRuleBase
{
    public string SourceDeviceId { get; init; } = null!;
    public string SourcePortId { get; init; } = null!;
    public TriggerRuleSourcePortStatus SourcePortStatus { get; init; } = null!;
}

public record TriggerRule : TriggerRuleBase
{
    public string DestinationDeviceId { get; init; } = null!;
    public string DestinationPortId { get; init; } = null!;
    public List<TriggerRuleBase> AdditionalConditions { get; init; } = new();
    public TriggerRuleMode Mode { get; init; }
    public TriggerRuleAction Action { get; init; } = null!;
    public TriggerResult Result { get; init; }

    // public PortStatusUpdateMode SourcePortStatusUpdateMode { get; init; }
    // public int IntervalRequestStatus { get; init; }
}

public enum TriggerRuleMode
{
    /// <summary> Event from device </summary>
    OnEvent,

    /// <summary> Request port status by scheduler </summary>
    ByRequestStatus,
}

public record TriggerRuleSourcePortStatus
{
    public IEnumerable<SWStatus> SWStatuses { get; init; } = Array.Empty<SWStatus>();
    public IEnumerable<DeviceInPortCommand> InPortCommands { get; init; } = Array.Empty<DeviceInPortCommand>();
}

public record TriggerRuleAction
{
    public int DelayForActionInSeconds { get; init; }
    public DeviceOutPortCommand? SWCommand { get; init; }
}

public enum TriggerResult
{
    Default,
    DoNothing,
}

public enum PortStatusUpdateMode
{
    Equal,
    More,
    Less,
}