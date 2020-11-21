using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IDeviceDataParser
    {
        DeviceEvent ParseEvent(string deviceId, List<(string key, string value)> query);
        
        DevicePortStatus ParseStatus(DevicePort port, string portStatus);
    }
}