using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services
{
    public class DeviceConnector : IDeviceConnector
    {
        private readonly HttpClient httpClient;
        private readonly IDevicePortStatusParser portStatusParser;
        
        public DeviceConnector(HttpClient httpClient, IDevicePortStatusParser portStatusParser)
        {
            this.httpClient = httpClient;
            this.portStatusParser = portStatusParser;
        }

        public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                var query = $"?pt={port.Id}&cmd=get";
                var data = await SendRequestAsync(port.Device, query);

                return portStatusParser.ParseStatus(port, data);
            });
        }

        private async Task<string> SendRequestAsync(Device device, string query)
        {
            var url = $"{device.Ip}/{device.Pwd}/{query}";
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await httpClient.SendAsync(httpRequest, cts.Token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync();
        }
    }
}