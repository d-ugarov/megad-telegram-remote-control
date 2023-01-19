using MegaDTelegramRemoteControl.Infrastructure.Configurations;
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
            case DeviceEventType.PortEvent:
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

        foreach (var triggerRule in deviceEvent.Event!.Port.TriggerRules.Where(x => x.Mode == TriggerRuleMode.OnEvent))
        {
            switch (deviceEvent.Event.Port.Type)
            {
                case DevicePortType.IN:
                {
                    result = await ProcessDeviceInEventAsync(deviceEvent, triggerRule);
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

    private async Task<TriggerResult> ProcessDeviceInEventAsync(DeviceEvent deviceEvent, DevicePortTriggerRule triggerRule)
    {
        if (!triggerRule.SourcePortStatus.InPortCommands.Contains(deviceEvent.Event!.In!.Command))
            return TriggerResult.Default;

        switch (triggerRule.DestinationPort.OutMode)
        {
            case DeviceOutPortMode.SW when triggerRule.Action.SWCommand.HasValue:
            {
                var command = DevicePortAction.CommandSW(triggerRule.Action.SWCommand.Value);
                await deviceConnector.InvokePortActionAsync(triggerRule.DestinationPort, command);
                
                logger.LogTrace($"Trigger: set command {triggerRule.Action.SWCommand.Value} port {triggerRule.DestinationPort}");
                
                return triggerRule.Result;
            }
        }

        return TriggerResult.Default;
    }
}