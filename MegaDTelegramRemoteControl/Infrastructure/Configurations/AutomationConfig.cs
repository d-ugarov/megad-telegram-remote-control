using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public class AutomationConfig
    {
        public List<TriggerRule> Triggers { get; set; } = new();
    }

    public class TriggerRule
    {
        public string SourceDeviceId { get; set; } = null!;
        public string SourcePortId { get; set; } = null!;
        public string SourcePortStatus { get; set; } = null!;
        public PortStatusUpdateMode SourcePortStatusUpdateMode { get; set; }
        public TriggerRuleMode Mode { get; set; }
        public int IntervalRequestStatus { get; set; }
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