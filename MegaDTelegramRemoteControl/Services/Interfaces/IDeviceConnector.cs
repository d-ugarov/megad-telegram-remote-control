using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IDeviceConnector
{
    Task<OperationResult<DevicePortInfo>> GetPortStatusAsync(DevicePort port);

    Task<OperationResult<List<DevicePortInfo>>> GetDevicePortsStatusesAsync(Device device);

    Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action);
}