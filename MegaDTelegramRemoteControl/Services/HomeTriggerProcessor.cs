﻿using MegaDTelegramRemoteControl.Infrastructure.Configurations;
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
        var deviceStatuses = new Dictionary<string, List<DevicePortInfo>>();

        foreach (var device in deviceEvent.Event!.Port.TriggerRulesRequiredDevices)
        {
            var statuses = await deviceConnector.GetDevicePortsStatusesAsync(device);
            if (statuses.IsSuccess)
                deviceStatuses.Add(device.Id, statuses.Data!);
        }

        foreach (var triggerRule in deviceEvent.Event!.Port.TriggerRules)
        {
            switch (deviceEvent.Event.Port.Type)
            {
                case DevicePortType.IN when triggerRule.SourcePortStatus
                                                       .InPortCommands
                                                       .Contains(deviceEvent.Event.In!.Command) &&
                                            CheckAdditionalConditionsAllowed(triggerRule.AdditionalConditions, deviceStatuses):
                {
                    foreach (var rule in triggerRule.DestinationPortRules)
                    {
                        if (rule.DelayBeforeAction.HasValue)
                            await Task.Delay(rule.DelayBeforeAction.Value);

                        await ProcessDeviceInEventAsync(rule, deviceStatuses);

                        if (rule.DelayAfterAction.HasValue)
                            await Task.Delay(rule.DelayAfterAction.Value);
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

    private static bool CheckAdditionalConditionsAllowed(AdditionalConditions? additionalConditions,
        IReadOnlyDictionary<string, List<DevicePortInfo>> deviceStatuses)
    {
        if (additionalConditions == null)
            return true;
        
        var statuses = new List<InOutSWStatus>();

        foreach (var conditionsPort in additionalConditions.Ports)
        {
            if (!deviceStatuses.TryGetValue(conditionsPort.Device.Id, out var states))
                continue;

            var state = states.FirstOrDefault(x => x.Port.Id == conditionsPort.Id);
            if (state != null)
            {
                statuses.Add(state.Status.InOutSwStatus);
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

    private async Task ProcessDeviceInEventAsync(DestinationTriggerRule triggerRule, IReadOnlyDictionary<string, List<DevicePortInfo>> deviceStatuses)
    {
        switch (triggerRule.Port.OutMode)
        {
            case DeviceOutPortMode.SW when triggerRule.Action.SWCommand.HasValue:
            {
                DevicePortInfo? portStatus = null;

                if (deviceStatuses.TryGetValue(triggerRule.Port.Device.Id, out var states))
                {
                    portStatus = states.FirstOrDefault(x => x.Port.Id == triggerRule.Port.Id);

                    if (portStatus != null && IsNewStatusEqual(portStatus.Status.InOutSwStatus, triggerRule.Action.SWCommand.Value))
                    {
                        logger.LogCritical($"Trigger: skip command {triggerRule.Action.SWCommand}, {triggerRule.Port}");
                        return;
                    }
                }

                var command = ImproveCommand(triggerRule.Action.SWCommand.Value, portStatus);
                var result = await deviceConnector.InvokePortActionAsync(triggerRule.Port, DevicePortAction.CommandSW(command));

                logger.LogCritical($"Trigger: command {triggerRule.Action.SWCommand}" +
                                   (triggerRule.Action.SWCommand != command ? $" ({command})" : "") +
                                   $", {triggerRule.Port}: " +
                                   $"{result.Report()}");
                break;
            }
        }
    }

    private static bool IsNewStatusEqual(InOutSWStatus currentState, DeviceOutPortCommand action) => currentState switch
    {
        InOutSWStatus.On when action == DeviceOutPortCommand.On => true,
        InOutSWStatus.Off when action == DeviceOutPortCommand.Off => true,
        _ => false,
    };
    
    private static DeviceOutPortCommand ImproveCommand(DeviceOutPortCommand command, DevicePortInfo? portStatus) => command switch
    {
        DeviceOutPortCommand.Switch when portStatus?.Status.InOutSwStatus == InOutSWStatus.Off => DeviceOutPortCommand.On,
        DeviceOutPortCommand.Switch when portStatus?.Status.InOutSwStatus == InOutSWStatus.On => DeviceOutPortCommand.Off,
        _ => command,
    };
}