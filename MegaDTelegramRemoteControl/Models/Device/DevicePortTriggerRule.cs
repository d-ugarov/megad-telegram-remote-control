using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortTriggerRule
{
    public required TriggerRuleSourcePortStatus SourcePortStatus { get; init; }
    public List<DestinationTriggerRule> DestinationPortRules { get; } = new();
    public required TriggerResult Result { get; init; }
}

public record DestinationTriggerRule
{
    public required DevicePort Port { get; init; }
    public required TriggerRuleAction Action { get; init; }
}