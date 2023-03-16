using SmartHome.Common.Models.Device;

namespace SmartHome.Common.Interfaces;

public interface IDeviceCommandParser
{
    DeviceEvent ParseEvent(IDevice<IDevicePort> device, DeviceEventRaw eventRaw);

    DevicePortInfo ParseStatus(IDevicePort devicePort, string portStatusRaw);
}