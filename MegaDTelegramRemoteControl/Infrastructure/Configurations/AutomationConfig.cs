using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public record AutomationConfig
    {
        public List<TriggerRule> Triggers { get; init; } = new();
    }

    public record TriggerRule
    {
        public string SourceDeviceId { get; init; } = null!;
        public string SourcePortId { get; init; } = null!;
        public string SourcePortStatus { get; init; } = null!;
        public PortStatusUpdateMode SourcePortStatusUpdateMode { get; init; }
        public TriggerRuleMode Mode { get; init; }
        public int IntervalRequestStatus { get; init; }
    }

    public enum TriggerRuleMode
    {
        OnEvent,
        ByRequestStatus,
    }

    public enum PortStatusUpdateMode
    {
        Equal,
        More,
        Less,
    }
}