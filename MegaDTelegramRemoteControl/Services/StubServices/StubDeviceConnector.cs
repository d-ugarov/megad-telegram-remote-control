using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.StubServices;

public class StubDeviceConnector : IDeviceConnector
{
    private readonly IDeviceCommandParser deviceDataParser;

    public StubDeviceConnector(IDeviceCommandParser deviceDataParser)
    {
        this.deviceDataParser = deviceDataParser;
    }

    public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port)
    {
        return Task.FromResult(InvokeOperations.InvokeOperation(() => GetRandomPortStatus(port)));
    }

    public Task<OperationResult<List<DevicePortStatus>>> GetDevicePortsStatusesAsync(Device device)
    {
        return Task.FromResult(InvokeOperations.InvokeOperation(() => device.Ports.Values.Select(GetRandomPortStatus).ToList()));
    }

    public Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action)
    {
        return Task.FromResult(OperationResult.Ok());
    }

    private DevicePortStatus GetRandomPortStatus(DevicePort port)
    {
        var bytes = new byte[8];

        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);

        var rand = BitConverter.ToInt64(bytes, 0);
        var data = rand % 2 == 0 ? InOutSWStatus.On : InOutSWStatus.Off;

        return deviceDataParser.ParseStatus(port, data.ToString());
    }
}