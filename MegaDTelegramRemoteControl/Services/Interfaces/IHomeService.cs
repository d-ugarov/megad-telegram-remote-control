using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Home;
using MegaDTelegramRemoteControl.Models.Scheduler;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IHomeService
{
    Dictionary<string, Device> Devices { get; }
    List<Location> Locations { get; }
    List<Scheduler> Schedulers { get; }

    List<UpdatedPortStatus> UpdateCurrentState(Device device, IEnumerable<DevicePortInfo> devicePortsInfos);

    HomePortStatus? GetPortStatus(DevicePort port);
}