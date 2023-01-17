using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services;

public class HomeLogic : IHomeLogic
{
    private readonly HomeConfig homeConfig;
    private readonly IHomeState homeState;
    private readonly IDeviceCommandParser deviceCommandParser;
    private readonly IDeviceConnector deviceConnector;
    private readonly IBotService botService;
    private readonly ILogger<HomeLogic> logger;

    public HomeLogic(HomeConfig homeConfig,
        IHomeState homeState,
        IDeviceCommandParser deviceCommandParser,
        IDeviceConnector deviceConnector,
        IBotService botService,
        ILogger<HomeLogic> logger)
    {
        this.homeConfig = homeConfig;
        this.homeState = homeState;
        this.deviceCommandParser = deviceCommandParser;
        this.deviceConnector = deviceConnector;
        this.botService = botService;
        this.logger = logger;
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

            homeState.Set(deviceEvent);

            return new NewEventResult();
        });
    }
}