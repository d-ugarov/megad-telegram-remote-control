using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface ITelegramService
    {
        Task InitBotAsync();
        
        Task SendDebugTextMessageAsync(string message);
    }
}