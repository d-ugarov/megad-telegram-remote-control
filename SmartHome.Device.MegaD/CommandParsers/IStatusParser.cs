using SmartHome.Common.Models.Device;

namespace SmartHome.Device.MegaD.CommandParsers;

internal interface IStatusParser
{
    DevicePortInfo Parse(IDevicePort devicePort, string portStatusRaw);
}