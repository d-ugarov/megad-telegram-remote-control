using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortTriggerRule
{
    public required TriggerRuleSourcePortStatus SourcePortStatus { get; init; }
    public AdditionalConditions? AdditionalConditions { get; set; }
    public List<DestinationTriggerRule> DestinationPortRules { get; } = new();
    public required TriggerResult Result { get; init; }
}

public record DestinationTriggerRule
{
    public required DevicePort Port { get; init; }
    public required TriggerRuleAction Action { get; init; }
}

public record AdditionalConditions
{
    public List<DevicePort> Ports { get; } = new();
    public required ConditionType Type { get; init; }
    public required SWStatus? Status { get; set; }
}