using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services
{
    public class InitService : IHostedService
    {
        private readonly ITelegramService telegramService;
        
        public InitService(ITelegramService telegramService)
        {
            this.telegramService = telegramService;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await telegramService.InitBotAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}