using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Models.Device;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.Common.Interfaces;

public interface IDeviceConnector
{
    Task<OperationResult<DevicePortInfo>> GetPortStatusAsync(IDevice device, IDevicePort port);

    Task<OperationResult<List<DevicePortInfo>>> GetPortsStatusesAsync(IDevice<IDevicePort> device);

    Task<OperationResult> InvokePortActionAsync(IDevice device, IDevicePort port, DevicePortAction action);
}