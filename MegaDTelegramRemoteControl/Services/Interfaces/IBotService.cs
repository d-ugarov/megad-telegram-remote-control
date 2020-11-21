using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IBotService
    {
        Task InitBotAsync();
        
        Task SendDebugTextMessageAsync(string message);
    }
}