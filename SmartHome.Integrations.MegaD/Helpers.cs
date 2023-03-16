using SmartHome.Common.Models.Device.Enums;
using System.Collections.Generic;

namespace SmartHome.Integrations.MegaD;

internal class Helpers
{
    static Helpers()
    {
        DeviceEventTypes = new()
                           {
                               {"st", DeviceEventType.DeviceStarted},
                               {"pt", DeviceEventType.PortEvent},
                           };
    }

    public static readonly Dictionary<string, DeviceEventType> DeviceEventTypes;
}