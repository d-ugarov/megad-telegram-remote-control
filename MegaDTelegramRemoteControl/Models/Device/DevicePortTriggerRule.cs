using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public record DevicePortTriggerRule
    {
        public TriggerRuleSourcePortStatus SourcePortStatus { get; init; } = null!;
        public List<AdditionalDevicePortTriggerRule> AdditionalConditions { get; init; } = new();
        public TriggerRuleMode Mode { get; init; }
        public TriggerRuleAction Action { get; init; } = null!;
    }

    public record AdditionalDevicePortTriggerRule
    {
        public DevicePort DevicePort { get; init; } = null!;
        public TriggerRuleSourcePortStatus SourcePortStatus { get; init; } = null!;
    }
}