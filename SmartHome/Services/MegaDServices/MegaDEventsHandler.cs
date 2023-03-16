using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SmartHome.Common.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.MegaDServices;

public class MegaDEventsHandler : IMegaDEventsHandler
{
    private readonly IDeviceCommandParser deviceCommandParser;
    private readonly IHomeProcessor homeProcessor;
    private readonly ILogger<MegaDEventsHandler> logger;

    public MegaDEventsHandler(IDeviceCommandParser deviceCommandParser,
        ILogger<MegaDEventsHandler> logger,
        IHomeProcessor homeProcessor)
    {
        this.deviceCommandParser = deviceCommandParser;
        this.logger = logger;
        this.homeProcessor = homeProcessor;
    }

    public Task<OperationResult<NewEventResult>> OnNewEventAsync(string deviceId, IReadOnlyCollection<NewEventData> eventData)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var deviceEvent = deviceCommandParser.ParseEvent(deviceId, eventData);
            logger.LogTrace(deviceEvent.ToString());

            logger.LogCritical(deviceEvent.ToString());

            if (!deviceEvent.IsParsedSuccessfully)
                return NewEventResult.Default;

            var triggerResult = await homeProcessor.ProcessDeviceEventAsync(deviceEvent);

            return triggerResult switch
            {
                TriggerResult.DoNothing => NewEventResult.DoNothing,
                _ => NewEventResult.Default,
            };
        });
    }
}