using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;
using System;

namespace SmartHome.Integrations.MegaD.CommandParsers;

internal class OutSwStatusParser : IStatusParser
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