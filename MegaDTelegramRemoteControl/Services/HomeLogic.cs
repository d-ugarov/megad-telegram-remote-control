using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services
{
    public class HomeLogic : IHomeLogic
    {
        private readonly HomeConfig homeConfig;
        private readonly IHomeState homeState;
        private readonly IDeviceDataParser deviceDataParser;
        private readonly IDeviceConnector deviceConnector;
        private readonly IBotService botService;
        private readonly ILogger<HomeLogic> logger;
        
        public HomeLogic(HomeConfig homeConfig,
            IHomeState homeState,
            IDeviceDataParser deviceDataParser,
            IDeviceConnector deviceConnector,
            IBotService botService,
            ILogger<HomeLogic> logger)
        {
            this.homeConfig = homeConfig;
            this.homeState = homeState;
            this.deviceDataParser = deviceDataParser;
            this.deviceConnector = deviceConnector;
            this.botService = botService;
            this.logger = logger;
        }

        public Task<OperationResult<OnNewEventResult>> OnNewEventAsync(string deviceId, List<(string key, string value)> query)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                var deviceEvent = deviceDataParser.ParseEvent(deviceId, query);
                logger.LogTrace(deviceEvent.ToString());
                
                // todo: add to message queue
                await botService.SendDebugTextMessageAsync(deviceEvent.ToString());

                if (!deviceEvent.IsParsedSuccessfully)
                    return OnNewEventResult.Default;

                homeState.Set(deviceEvent);
                
                return new OnNewEventResult();
            });
        }
    }
}