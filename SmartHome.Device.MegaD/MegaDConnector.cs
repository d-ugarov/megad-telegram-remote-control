using SmartHome.Common.Infrastructure.Helpers;
using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Interfaces;
using SmartHome.Common.Models.Device;
using SmartHome.Common.Models.Device.Enums;

namespace SmartHome.Device.MegaD;

internal class MegaDConnector : IDeviceConnector
{
    private readonly HttpClient httpClient;
    private readonly IDeviceCommandParser deviceCommandParser;

    private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(2);

    public MegaDConnector(HttpClient httpClient, IDeviceCommandParser deviceCommandParser)
    {
        this.httpClient = httpClient;
        this.deviceCommandParser = deviceCommandParser;
    }

    public Task<OperationResult<DevicePortInfo>> GetPortStatusAsync(IDevice device, IDevicePort port)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var data = await SendRequestAsync(device, $"?pt={port.Id}&cmd=get");
            return deviceCommandParser.ParseStatus(port, data);
        });
    }

    public Task<OperationResult<List<DevicePortInfo>>> GetPortsStatusesAsync(IDevice<IDevicePort> device)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var data = await SendRequestAsync(device, "?cmd=all");
            var result = new List<DevicePortInfo>();

            var statuses = data.Split(';');

            for (var i = 0; i < statuses.Length; i++)
            {
                if (!device.Ports.TryGetValue(i, out var port))
                    continue;

                result.Add(deviceCommandParser.ParseStatus(port, statuses[i]));
            }

            return result;
        });
    }

    public Task<OperationResult> InvokePortActionAsync(IDevice device, IDevicePort port, DevicePortAction action)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            switch (port.Type)
            {
                case DevicePortType.OUT:
                {
                    switch (port.OutMode)
                    {
                        case DevicePortOutMode.SW when action.CommandSW.HasValue:
                        {
                            await SendRequestAsync(device, $"?cmd={port.Id}:{(int)action.CommandSW}");
                            break;
                        }
                    }

                    break;
                }
            }
        });
    }

    private async Task<string> SendRequestAsync(IDevice device, string query)
    {
        var content = await httpClient.SendRequestAsync($"{device.Ip}/{device.Pwd}/{query}", HttpMethod.Get, defaultTimeout);
        return await content.ReadAsStringAsync();
    }
}