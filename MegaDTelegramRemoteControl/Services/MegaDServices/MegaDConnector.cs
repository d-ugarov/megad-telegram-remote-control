using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services.MegaDServices
{
    public class MegaDConnector : IDeviceConnector
    {
        private readonly HttpClient httpClient;
        private readonly IDeviceCommandParser deviceCommandParser;
        
        public MegaDConnector(HttpClient httpClient, IDeviceCommandParser deviceCommandParser)
        {
            this.httpClient = httpClient;
            this.deviceCommandParser = deviceCommandParser;
        }

        public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                var query = $"?pt={port.Id}&cmd=get";
                var data = await SendRequestAsync(port.Device, query);

                return deviceCommandParser.ParseStatus(port, data);
            });
        }

        public Task<OperationResult> InvokePortActionAsync(DevicePort port, DevicePortAction action)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                switch (port.OutMode)
                {
                    case DeviceOutPortMode.SW when action.SWCommand.HasValue:
                    {
                        var query = $"?cmd={port.Id}:{(int)action.SWCommand}";
                        await SendRequestAsync(port.Device, query);
                        break;
                    }
                }
            });
        }
        
        private async Task<string> SendRequestAsync(Device device, string query)
        {
            var url = $"{device.Ip}/{device.Pwd}/{query}";
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await httpClient.SendAsync(httpRequest, cts.Token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync(cts.Token);
        }
    }
}