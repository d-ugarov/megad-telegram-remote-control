using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IHomeService
{
    Dictionary<string, Device> Devices { get; }
    List<Location> Locations { get; }

    void Set(DeviceEvent deviceEvent);
    DevicePortEvent? Get(DevicePort port);
    void Clean();
}