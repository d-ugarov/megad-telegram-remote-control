using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IMegaDEventsHandler
{
    Task<OperationResult<NewEventResult>> OnNewEventAsync(string deviceId, IReadOnlyCollection<NewEventData> eventData);
}