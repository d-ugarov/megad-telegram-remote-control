using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IDeviceEventParser
    {
        DeviceEvent ParseEvent(string deviceId, IReadOnlyCollection<(string key, string value)> eventData);
        
        DevicePortStatus ParseStatus(DevicePort port, string portStatus);
    }
}