using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;
using System;

namespace SmartHome.Integrations.MegaD.CommandParsers;

internal class InStatusParser : IStatusParser
{
    public DevicePortInfo Parse(IDevicePort devicePort, string portStatusRaw)
    {
        var inStatus = portStatusRaw.Split('/');
        var counter = inStatus.Length > 1 && int.TryParse(inStatus[1], out var value)
            ? value
            : 0;

        return new(devicePort)
               {
                   StatusIn = new DevicePortStatusIn
                              {
                                  InOutSwStatus = Enum.Parse<DevicePortInOutSWStatus>(inStatus[0], true),
                                  InCounter = counter,
                              }
               };
    }
}