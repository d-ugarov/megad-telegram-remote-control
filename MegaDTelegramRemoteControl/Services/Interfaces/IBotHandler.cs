using MegaDTelegramRemoteControl.Models;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IBotHandler
    {
        Task<BotMenu> ProcessActionAsync(string? actionId = null);
    }
}