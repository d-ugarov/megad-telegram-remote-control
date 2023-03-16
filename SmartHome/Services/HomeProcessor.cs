using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Models.Home;
using MegaDTelegramRemoteControl.Models.Interfaces;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SmartHome.Common.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;

namespace MegaDTelegramRemoteControl.Services;

public class HomeProcessor : IHomeProcessor
{
    private readonly IHomeService homeService;
    private readonly IDeviceConnector deviceConnector;
    private readonly ILogger<HomeProcessor> logger;

    public HomeProcessor(IDeviceConnector deviceConnector, IHomeService homeService, ILogger<HomeProcessor> logger)
    {
        this.deviceConnector = deviceConnector;
        this.homeService = homeService;
        this.logger = logger;
    }

    #region Device events

    public Task<TriggerResult> ProcessDeviceEventAsync(DeviceEvent deviceEvent)
    {
        switch (deviceEvent.Type)
        {
            case DeviceEventType.PortEvent when deviceEvent.Event!.Port.TriggerRules.Any():
            {
                return ProcessPortEventAsync(deviceEvent);
            }
            case DeviceEventType.DeviceStarted:
                break;
        }

        return Task.FromResult(TriggerResult.Default);
    }

    private async Task<TriggerResult> ProcessPortEventAsync(DeviceEvent deviceEvent)
    {
        var result = TriggerResult.Default;

        await UpdateHomeStateAsync(deviceEvent.Event!.Port.TriggerRulesRequiredDevices);

        foreach (var triggerRule in deviceEvent.Event!.Port.TriggerRules)
        {
            switch (deviceEvent.Event.Port.Type)
            {
                case DevicePortType.IN when triggerRule.SourcePortStatus
                                                       .InPortCommands
                                                       .Contains(deviceEvent.Event.In!.Command) &&
                                            IsConditionsAllowed(triggerRule.AdditionalConditions):
                {
                    foreach (var rule in triggerRule.DestinationPortRules)
                    {
                        if (rule.DelayBeforeAction.HasValue)
                            await Task.Delay(rule.DelayBeforeAction.Value);

                        await ExecuteRuleAsync(rule, ExecutionRuleMode.Trigger);

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

    #endregion

    #region Schedulers

    public async Task<List<UpdatedPortStatus>> UpdateHomeStateAsync(IEnumerable<Device>? selectedDevices = null)
    {
        var updatedStatuses = new List<UpdatedPortStatus>();

        foreach (var device in selectedDevices ?? homeService.Devices.Values)
        {
            var statuses = await deviceConnector.GetDevicePortsStatusesAsync(device);
            if (statuses.IsSuccess)
            {
                updatedStatuses.AddRange(homeService.UpdateCurrentState(device, statuses.Data!));
            }
        }

        return updatedStatuses;
    }

    public async Task ProcessSchedulerRulesAsync(List<UpdatedPortStatus> updatedStatuses)
    {
        foreach (var scheduler in homeService.Schedulers)
        {
            var isSpecificCheckAllowed = false;

            switch (scheduler.Type)
            {
                case SchedulerType.StartOnTime:
                {
                    // todo
                    break;
                }
                case SchedulerType.StartByCondition when scheduler.Conditions != null:
                {
                    isSpecificCheckAllowed = scheduler.Conditions!
                                                      .Ports
                                                      .Any(x => updatedStatuses.Any(s => s.Port == x));
                    break;
                }
            }

            if (!isSpecificCheckAllowed)
                continue;

            // todo: check period
            
            if (scheduler.Conditions != null && !IsConditionsAllowed(scheduler.Conditions))
                continue;

            if (scheduler.DestinationPortRules.All(IsRuleStatusEqual))
                continue;

            foreach (var rule in scheduler.DestinationPortRules)
            {
                await ExecuteRuleAsync(rule, ExecutionRuleMode.Scheduler);
            }
        }
    }

    #endregion

    #region Common

    private bool IsConditionsAllowed(IConditions? conditions)
    {
        if (conditions == null)
            return true;

        var portStatuses = conditions.Ports
                                     .Select(x => homeService.GetPortStatus(x))
                                     .Where(x => x != null)
                                     .ToList();

        var statuses = portStatuses.Select(x => x!.CurrentStatus.InOutSwStatus);

        return conditions.Type switch
        {
            ConditionType.StatesAreEqual when conditions.Status.HasValue => statuses.All(x => x == conditions.Status.Value),
            ConditionType.StatesAreEqual => statuses.Distinct().Count() == 1,

            ConditionType.StatesAnyEqual when conditions.Status.HasValue => statuses.Any(x => x == conditions.Status.Value),
            ConditionType.StatesAnyEqual => statuses.Distinct().Count() == 1,

            ConditionType.StatesNotEqual when conditions.Status.HasValue => statuses.All(x => x != conditions.Status.Value),
            ConditionType.StatesNotEqual => statuses.Distinct().Count() > 1,

            _ => true,
        };
    }

    private async Task ExecuteRuleAsync(IDestinationRule rule, ExecutionRuleMode mode)
    {
        switch (rule.Port.OutMode)
        {
            case DeviceOutPortMode.SW when rule.Action.SWCommand.HasValue:
            {
                var portStatus = homeService.GetPortStatus(rule.Port);

                if (portStatus != null && IsNewStatusEqual(portStatus.CurrentStatus.InOutSwStatus, rule.Action.SWCommand.Value))
                {
                    logger.LogCritical($"{mode}: skip command {rule.Action.SWCommand}, {rule.Port}");
                    return;
                }

                var command = ImproveCommand(rule.Action.SWCommand.Value, portStatus?.CurrentStatus);
                var result = await deviceConnector.InvokePortActionAsync(rule.Port, DevicePortAction.CommandSW(command));

                logger.LogCritical($"{mode}: command {rule.Action.SWCommand}" +
                                   (rule.Action.SWCommand != command ? $" ({command})" : "") +
                                   $", {rule.Port}: " +
                                   $"{result.Report()}");
                break;
            }
        }
    }

    private bool IsRuleStatusEqual(IDestinationRule rule)
    {
        switch (rule.Port.OutMode)
        {
            case DeviceOutPortMode.SW when rule.Action.SWCommand.HasValue:
            {
                var portStatus = homeService.GetPortStatus(rule.Port);

                if (portStatus != null && IsNewStatusEqual(portStatus.CurrentStatus.InOutSwStatus, rule.Action.SWCommand.Value))
                    return false;

                break;
            }
        }

        return true;
    }

    private static bool IsNewStatusEqual(InOutSWStatus currentState, DeviceOutPortCommand action) => currentState switch
    {
        InOutSWStatus.On when action == DeviceOutPortCommand.On => true,
        InOutSWStatus.Off when action == DeviceOutPortCommand.Off => true,
        _ => false,
    };
    
    private static DeviceOutPortCommand ImproveCommand(DeviceOutPortCommand command, DevicePortStatus? portStatus) => command switch
    {
        DeviceOutPortCommand.Switch when portStatus?.InOutSwStatus == InOutSWStatus.Off => DeviceOutPortCommand.On,
        DeviceOutPortCommand.Switch when portStatus?.InOutSwStatus == InOutSWStatus.On => DeviceOutPortCommand.Off,
        _ => command,
    };

    #endregion
}