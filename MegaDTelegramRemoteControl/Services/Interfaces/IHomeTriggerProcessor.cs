using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IHomeTriggerProcessor
{
    Task<TriggerResult> ProcessAsync(DeviceEvent deviceEvent);
}