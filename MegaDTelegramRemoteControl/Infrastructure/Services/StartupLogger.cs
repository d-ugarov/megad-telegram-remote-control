using Microsoft.Extensions.Logging;

namespace MegaDTelegramRemoteControl.Infrastructure.Services
{
    public class StartupLogger
    {
        private readonly ILogger<StartupLogger> logger;

        public StartupLogger(ILogger<StartupLogger> logger)
        {
            this.logger = logger;
        }

        public void Log(string message)
        {
            logger.LogInformation(message);
        }
    }
}