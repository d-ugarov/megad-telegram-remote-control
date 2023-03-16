using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using SmartHome.Common.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public static class Constants
{
    static Constants()
    {
        InstanceId = Guid.NewGuid();
        DeviceEventTypes = Extensions.GetEnumList<DeviceEventType>()
                                     .ToDictionary(x => x.GetAttribute<DeviceEventAttribute>().Command, x => x);
    }

    public static readonly Guid InstanceId;

    public static readonly Dictionary<string, DeviceEventType> DeviceEventTypes;
}