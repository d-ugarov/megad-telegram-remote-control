using MegaDTelegramRemoteControl.Services.Interfaces;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services;

public class InitService : IWarmupCacheService
{
    private readonly IBotService botService;

    public InitService(IBotService botService)
    {
        this.botService = botService;
    }

    public bool IsCacheWarmedUp { get; private set; }

    public async Task InitCacheAsync(bool isPrimaryCache)
    {
        if (isPrimaryCache || IsCacheWarmedUp)
            return;

        await botService.InitBotAsync();
        IsCacheWarmedUp = true;
    }
}