using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IDeviceCommandParser
{
    DeviceEvent ParseEvent(string deviceId, IReadOnlyCollection<NewEventData> eventData);

    DevicePortStatus ParseStatus(DevicePort port, string portStatus);
}