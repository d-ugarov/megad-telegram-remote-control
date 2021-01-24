using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IHomeLogic
    {
        Task<OperationResult<OnNewEventResult>> OnNewEventAsync(string deviceId, IReadOnlyCollection<(string key, string value)> eventData);
    }
}