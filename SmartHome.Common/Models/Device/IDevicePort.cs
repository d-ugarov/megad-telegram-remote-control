using SmartHome.Common.Models.Device.Enums;

namespace SmartHome.Common.Models.Device;

public interface IDevicePort
{
    int Id { get; }
    DevicePortType Type { get; }
    DevicePortOutMode? OutMode { get; }
}