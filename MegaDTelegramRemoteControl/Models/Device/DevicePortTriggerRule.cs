using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortTriggerRule
{
    public required TriggerRuleSourcePortStatus SourcePortStatus { get; init; }
    public required TriggerRuleMode Mode { get; init; }
    public required TriggerRuleAction Action { get; init; }
    public required TriggerResult Result { get; init; }
    public required DevicePort DestinationPort { get; init; }
    public List<AdditionalDevicePortTriggerRule> AdditionalConditions { get; init; } = new();
}

public record AdditionalDevicePortTriggerRule
{
    public required DevicePort DevicePort { get; init; }
    public required TriggerRuleSourcePortStatus SourcePortStatus { get; init; }
}