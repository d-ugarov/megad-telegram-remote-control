using SmartHome.Common.Models.Device;

namespace SmartHome.Integrations.MegaD.CommandParsers;

internal interface IStatusParser
{
    DevicePortInfo Parse(IDevicePort devicePort, string portStatusRaw);
}