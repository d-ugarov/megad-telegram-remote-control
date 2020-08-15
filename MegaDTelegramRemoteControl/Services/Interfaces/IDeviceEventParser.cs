using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IDeviceEventParser
    {
        DeviceEvent ParseEvent(string deviceId, List<(string key, string value)> query);
    }
}