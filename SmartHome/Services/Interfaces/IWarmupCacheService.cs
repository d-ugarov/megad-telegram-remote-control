using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IWarmupCacheService
{
    bool IsCacheWarmedUp { get; }

    Task InitCacheAsync(bool isPrimaryCache);
}