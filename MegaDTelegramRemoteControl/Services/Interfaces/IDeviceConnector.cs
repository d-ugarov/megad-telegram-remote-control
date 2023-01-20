using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IDeviceConnector
{
    Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port, bool useCache = false);

    Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action);
}