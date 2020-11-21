using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IBotHandler
    {
        Task<OperationResult<BotMenu>> ProcessActionAsync(string actionId = null);
    }
}