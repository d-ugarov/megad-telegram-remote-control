using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services
{
    public class DeviceConnector : IDeviceConnector
    {
        private readonly HttpClient httpClient;
        
        public DeviceConnector(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port)
        {
            return InvokeOperations.InvokeOperationAsync(() =>
            {
                return Task.FromResult(new DevicePortStatus());
            });
        }
    }
}