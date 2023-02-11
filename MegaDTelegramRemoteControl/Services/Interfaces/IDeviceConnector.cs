using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IDeviceConnector
{
    Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port, bool useCache = false);

    Task<OperationResult<List<DevicePortStatus>>> GetPortStatusesAsync(Device device);

    Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action);
}