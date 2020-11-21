using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services
{
    public class InitService : IHostedService
    {
        private readonly IBotService botService;
        
        public InitService(IBotService botService)
        {
            this.botService = botService;
        }
        
        public Task StartAsync(CancellationToken cancellationToken) => botService.InitBotAsync();

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}