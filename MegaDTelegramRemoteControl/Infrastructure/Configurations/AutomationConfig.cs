using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public record AutomationConfig
    {
        public List<TriggerRule> Triggers { get; init; } = new();
    }

    public record BaseTriggerRule
    {
        public string SourceDeviceId { get; init; } = null!;
        public string SourcePortId { get; init; } = null!;
        public TriggerRuleSourcePortStatus SourcePortStatus { get; init; } = null!;
    }

    public record TriggerRule : BaseTriggerRule
    {
        public string DestinationDeviceId { get; init; } = null!;
        public string DestinationPortId { get; init; } = null!;
        public List<BaseTriggerRule> AdditionalConditions { get; init; } = new();
        public TriggerRuleMode Mode { get; init; }
        public TriggerRuleAction Action { get; init; } = null!;

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
        public SWStatus? SWStatus { get; init; }
    }

    public record TriggerRuleAction
    {
        public int DelayForActionInSeconds { get; init; }
        public DeviceOutPortCommand? SWCommand { get; init; }
    }

    public enum PortStatusUpdateMode
    {
        Equal,
        More,
        Less,
    }
}