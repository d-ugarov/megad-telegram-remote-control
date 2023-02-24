using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services.MegaDServices;

public class MegaDConnector : IDeviceConnector
{
    private readonly HttpClient httpClient;
    private readonly IDeviceCommandParser deviceCommandParser;

    public MegaDConnector(HttpClient httpClient,
        IDeviceCommandParser deviceCommandParser)
    {
        this.httpClient = httpClient;
        this.deviceCommandParser = deviceCommandParser;
    }

    public Task<OperationResult<DevicePortInfo>> GetPortStatusAsync(DevicePort port)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var url = GetUrl(port.Device, $"?pt={port.Id}&cmd=get");
            var data = await SendRequestAsync(url);
            return deviceCommandParser.ParseStatus(port, data);
        });
    }

    public Task<OperationResult<List<DevicePortInfo>>> GetDevicePortsStatusesAsync(Device device)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var url = GetUrl(device, "?cmd=all");
            var data = await SendRequestAsync(url);
            var result = new List<DevicePortInfo>();

            var statuses = data.Split(';');

            for (var i = 0; i < statuses.Length; i++)
            {
                if (!device.Ports.TryGetValue(i.ToString(), out var port))
                    continue;

                result.Add(deviceCommandParser.ParseStatus(port, statuses[i]));
            }

            return result;
        });
    }

    public Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            switch (port.Type)
            {
                case DevicePortType.OUT:
                {
                    switch (port.OutMode)
                    {
                        case DeviceOutPortMode.SW when action.SWCommand.HasValue:
                        {
                            var url = GetUrl(port.Device, $"?cmd={port.Id}:{(int)action.SWCommand}");
                            await SendRequestAsync(url);
                            break;
                        }
                    }

                    break;
                }
            }
        });
    }

    private async Task<string> SendRequestAsync(string url)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await httpClient.SendAsync(httpRequest, cts.Token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cts.Token);
    }

    private static string GetUrl(Device device, string query) => $"{device.Ip}/{device.Pwd}/{query}";
}