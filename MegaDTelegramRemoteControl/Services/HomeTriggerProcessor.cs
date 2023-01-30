using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services;

public class HomeTriggerProcessor : IHomeTriggerProcessor
{
    private readonly IDeviceConnector deviceConnector;
    private readonly ILogger<HomeTriggerProcessor> logger;

    public HomeTriggerProcessor(IDeviceConnector deviceConnector, ILogger<HomeTriggerProcessor> logger)
    {
        this.deviceConnector = deviceConnector;
        this.logger = logger;
    }

    public Task<TriggerResult> ProcessAsync(DeviceEvent deviceEvent)
    {
        switch (deviceEvent.Type)
        {
            case DeviceEventType.PortEvent when deviceEvent.Event!.Port.TriggerRules.Any():
            {
                return ProcessDeviceEventAsync(deviceEvent);
            }
            case DeviceEventType.DeviceStarted:
                break;
        }

        return Task.FromResult(TriggerResult.Default);
    }

    private async Task<TriggerResult> ProcessDeviceEventAsync(DeviceEvent deviceEvent)
    {
        var result = TriggerResult.Default;

        foreach (var triggerRule in deviceEvent.Event!.Port.TriggerRules)
        {
            switch (deviceEvent.Event.Port.Type)
            {
                case DevicePortType.IN when triggerRule.SourcePortStatus
                                                       .InPortCommands
                                                       .Contains(deviceEvent.Event.In!.Command) &&
                                            await CheckAdditionalConditionsAllowedAsync(triggerRule.AdditionalConditions):
                {
                    foreach (var destinationPortRule in triggerRule.DestinationPortRules)
                    {
                        if (destinationPortRule.DelayBeforeAction.HasValue)
                            await Task.Delay(destinationPortRule.DelayBeforeAction.Value);

                        await ProcessDeviceInEventAsync(destinationPortRule);

                        if (destinationPortRule.DelayAfterAction.HasValue)
                            await Task.Delay(destinationPortRule.DelayAfterAction.Value);
                    }

                    result = triggerRule.Result;

                    if (triggerRule.IsFinal)
                        return result;

                    break;
                }
                case DevicePortType.OUT:
                    break;
                default:
                    continue;
            }
        }

        return result;
    }

    private async Task<bool> CheckAdditionalConditionsAllowedAsync(AdditionalConditions? additionalConditions)
    {
        if (additionalConditions == null)
            return true;
        
        var statuses = new List<SWStatus>();

        foreach (var conditionsPort in additionalConditions.Ports)
        {
            var state = await deviceConnector.GetPortStatusAsync(conditionsPort, true);
            if (state.IsSuccess)
            {
                statuses.Add(state.Data!.SWStatus);
            }
        }

        return additionalConditions.Type switch
        {
            ConditionType.StatesAreEqual when additionalConditions.Status.HasValue => statuses.All(x => x == additionalConditions.Status.Value),
            ConditionType.StatesAreEqual => statuses.Distinct().Count() == 1,

            ConditionType.StatesAnyEqual when additionalConditions.Status.HasValue => statuses.Any(x => x == additionalConditions.Status.Value),
            ConditionType.StatesAnyEqual => statuses.Distinct().Count() == 1,

            ConditionType.StatesNotEqual when additionalConditions.Status.HasValue => statuses.All(x => x != additionalConditions.Status.Value),
            ConditionType.StatesNotEqual => statuses.Distinct().Count() > 1,

            _ => true,
        };
    }

    private async Task ProcessDeviceInEventAsync(DestinationTriggerRule triggerRule)
    {
        switch (triggerRule.Port.OutMode)
        {
            case DeviceOutPortMode.SW when triggerRule.Action.SWCommand.HasValue:
            {
                var command = DevicePortAction.CommandSW(triggerRule.Action.SWCommand.Value);
                var result = await deviceConnector.InvokePortActionAsync(triggerRule.Port, command);

                logger.LogTrace($"Trigger: command {triggerRule.Action.SWCommand.Value}, " +
                                $"{triggerRule.Port}: " +
                                $"{result.Report()}");
                logger.LogCritical($"Trigger: command {triggerRule.Action.SWCommand.Value}, " +
                                   $"{triggerRule.Port}: " +
                                   $"{result.Report()}");

                break;
            }
        }
    }
}