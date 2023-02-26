using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Home;
using System.Collections.Generic;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IHomeProcessor
{
    Task<TriggerResult> ProcessDeviceEventAsync(DeviceEvent deviceEvent);

    Task<List<UpdatedPortStatus>> UpdateHomeStateAsync(IEnumerable<Device>? selectedDevices = null);

    Task ProcessSchedulerRulesAsync(List<UpdatedPortStatus> updatedStatuses);
}