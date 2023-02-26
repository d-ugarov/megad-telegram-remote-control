using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Home;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services;

public class HomeService : IHomeService
{
    private readonly Dictionary<Device, Dictionary<DevicePort, HomePortStatus>> currentState;
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

    public void UpdateCurrentState(Device device, IEnumerable<DevicePortInfo> devicePortsInfos)
    {
        if (!currentState.TryGetValue(device, out var values))
        {
            values = new();
            currentState.Add(device, values);
        }

        foreach (var info in devicePortsInfos)
        {
            if (values.TryGetValue(info.Port, out var value))
            {
                if (!value.IsEqual(info.Port, info.Status))
                {
                    value.SetNewStatus(info.Status);
                }
            }
            else
            {
                values.Add(info.Port, new(info.Status));
            }
        }
    }
}