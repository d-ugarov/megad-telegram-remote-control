﻿using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortTriggerRule
{
    public required TriggerRuleSourcePortStatus SourcePortStatus { get; init; }
    public AdditionalConditions? AdditionalConditions { get; set; }
    public List<DestinationTriggerRule> DestinationPortRules { get; } = new();
    public required TriggerResult Result { get; init; }
    public required bool IsFinal { get; init; }
}

public record DestinationTriggerRule : IDestinationRule
{
    public required DevicePort Port { get; init; }
    public required TriggerRuleAction Action { get; init; }
    public required TimeSpan? DelayBeforeAction { get; init; }
    public required TimeSpan? DelayAfterAction { get; init; }
}

public record AdditionalConditions : IConditions
{
    public List<DevicePort> Ports { get; } = new();
    public required ConditionType Type { get; init; }
    public required InOutSWStatus? Status { get; init; }
}