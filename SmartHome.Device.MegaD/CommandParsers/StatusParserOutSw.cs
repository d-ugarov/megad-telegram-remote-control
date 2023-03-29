using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;

namespace SmartHome.Device.MegaD.CommandParsers;

internal class StatusParserOutSw : IStatusParser
{
    public DevicePortInfo Parse(IDevicePort devicePort, string portStatusRaw)
    {
        return new(devicePort)
               {
                   StatusOut = new DevicePortStatusOut
                               {
                                   InOutSwStatus = Enum.Parse<DevicePortInOutSWStatus>(portStatusRaw, true),
                               }
               };
    }
}