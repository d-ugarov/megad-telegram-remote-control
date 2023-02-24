﻿using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.MegaDServices;

public class MegaDEventsHandler : IMegaDEventsHandler
{
    private readonly IDeviceCommandParser deviceCommandParser;
    private readonly IHomeTriggerProcessor homeTriggerProcessor;
    private readonly ILogger<MegaDEventsHandler> logger;

    public MegaDEventsHandler(IDeviceCommandParser deviceCommandParser,
        ILogger<MegaDEventsHandler> logger,
        IHomeTriggerProcessor homeTriggerProcessor)
    {
        this.deviceCommandParser = deviceCommandParser;
        this.logger = logger;
        this.homeTriggerProcessor = homeTriggerProcessor;
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

            var triggerResult = await homeTriggerProcessor.ProcessAsync(deviceEvent);

            return triggerResult switch
            {
                TriggerResult.DoNothing => NewEventResult.DoNothing,
                _ => NewEventResult.Default,
            };
        });
    }
}