using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Home;
using MegaDTelegramRemoteControl.Models.Scheduler;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services;

public class HomeService : IHomeService
{
    private readonly ConcurrentDictionary<Device, ConcurrentDictionary<DevicePort, HomePortStatus>> currentState;
    private readonly HomeConfig homeConfig;

    private readonly ILogger<HomeService> logger;

    public HomeService(HomeConfig homeConfig, ILogger<HomeService> logger)
    {
        currentState = new();

        this.logger = logger;
        this.homeConfig = homeConfig;
    }

    public Dictionary<string, Device> Devices => homeConfig.Devices;

    public List<Location> Locations => homeConfig.Locations;

    public List<Scheduler> Schedulers => homeConfig.Schedulers;

    public List<UpdatedPortStatus> UpdateCurrentState(Device device, IEnumerable<DevicePortInfo> devicePortsInfos)
    {
        var result = new List<UpdatedPortStatus>();

        if (!currentState.TryGetValue(device, out var deviceStates))
        {
            deviceStates = new();
            currentState[device] = deviceStates;
        }

        foreach (var info in devicePortsInfos)
        {
            if (deviceStates.TryGetValue(info.Port, out var portState))
            {
                if (!portState.IsEqual(info.Port, info.Status))
                {
                    portState.SetNewStatus(info.Status);
                    result.Add(new(portState, info.Port));
                }
            }
            else
            {
                portState = new HomePortStatus(info.Status);
                deviceStates[info.Port] = portState;
                result.Add(new(portState, info.Port));
            }
        }

        return result;
    }

    public HomePortStatus? GetPortStatus(DevicePort port)
    {
        return currentState.TryGetValue(port.Device, out var ports) &&
               ports.TryGetValue(port, out var portStatus)
            ? portStatus
            : null;
    }
}