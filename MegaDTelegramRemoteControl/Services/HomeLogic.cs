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
        private readonly IDeviceEventParser deviceEventParser;
        private readonly IDeviceConnector deviceConnector;
        private readonly ITelegramService telegramService;
        private readonly ILogger<HomeLogic> logger;
        
        public HomeLogic(HomeConfig homeConfig,
            IDeviceEventParser deviceEventParser,
            IDeviceConnector deviceConnector,
            ITelegramService telegramService,
            ILogger<HomeLogic> logger)
        {
            this.homeConfig = homeConfig;
            this.deviceEventParser = deviceEventParser;
            this.deviceConnector = deviceConnector;
            this.telegramService = telegramService;
            this.logger = logger;
        }

        public Task<OperationResult<OnNewEventResult>> OnNewEventAsync(string deviceId, List<(string key, string value)> query)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                var deviceEvent = deviceEventParser.ParseEvent(deviceId, query);
                logger.LogTrace(deviceEvent.ToString());
                
                // todo: add to message queue
                await telegramService.SendDebugTextMessageAsync(deviceEvent.ToString());
                
                return new OnNewEventResult();
            });
        }
    }
}