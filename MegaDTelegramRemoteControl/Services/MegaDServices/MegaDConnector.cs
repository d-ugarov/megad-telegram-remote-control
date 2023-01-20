using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services.MegaDServices;

public class MegaDConnector : IDeviceConnector
{
    private readonly HttpClient httpClient;
    private readonly PlatformConfig platformConfig;
    private readonly IMemoryCache memoryCache;
    private readonly IDeviceCommandParser deviceCommandParser;

    public MegaDConnector(HttpClient httpClient,
        IDeviceCommandParser deviceCommandParser,
        PlatformConfig platformConfig,
        IMemoryCache memoryCache)
    {
        this.httpClient = httpClient;
        this.deviceCommandParser = deviceCommandParser;
        this.platformConfig = platformConfig;
        this.memoryCache = memoryCache;
    }

    public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port, bool useCache = false)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var url = GetUrl(port.Device, $"?pt={port.Id}&cmd=get");
            var cacheKey = GetCacheKey(url);
            string data;

            if (useCache &&
                platformConfig.CachePortStatusesInSeconds > 0 &&
                memoryCache.TryGetValue(cacheKey, out string? cachedData) &&
                !string.IsNullOrEmpty(cachedData))
            {
                data = cachedData;
            }
            else
            {
                data = await SendRequestAsync(url);
                memoryCache.Set(cacheKey, data, TimeSpan.FromSeconds(platformConfig.CachePortStatusesInSeconds));
            }

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
                    var url = GetUrl(port.Device, $"?cmd={port.Id}:{(int)action.SWCommand}");
                    await SendRequestAsync(url);
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
    private static string GetCacheKey(string url) => $"Cache:Port:{url}";
}