using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services;

public class HomeLogicTriggerProcessor : IHomeLogicTriggerProcessor
{
    private readonly IDeviceConnector deviceConnector;
    private readonly ILogger<HomeLogicTriggerProcessor> logger;

    public HomeLogicTriggerProcessor(IDeviceConnector deviceConnector, ILogger<HomeLogicTriggerProcessor> logger)
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
                                                       .Contains(deviceEvent.Event.In!.Command):
                {
                    foreach (var destinationPortRule in triggerRule.DestinationPortRules)
                    {
                        await ProcessDeviceInEventAsync(destinationPortRule);
                    }

                    result = triggerRule.Result;
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