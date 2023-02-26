using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;
using System.Collections.Generic;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Models.Scheduler;

public record Scheduler
{
    public required string Title { get; init; }
    public required SchedulerType Type { get; init; }
    public SchedulerConditions? Conditions { get; set; }
    public List<SchedulerDestinationPortRule> DestinationPortRules { get; } = new();
}
    
public record SchedulerConditions
{
    public List<DevicePort> Ports { get; } = new();
    public required ConditionType Type { get; init; }
    public required InOutSWStatus? Status { get; init; }
    public TimeSpan Period { get; init; }
}

public record SchedulerDestinationPortRule
{
    public required DevicePort Port { get; init; }
    public required TriggerRuleAction Action { get; init; }
}