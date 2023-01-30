using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Infrastructure.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestLoggingMiddleware> logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var id = Guid.NewGuid();
        var ip = context.Connection.RemoteIpAddress?.ToString();

        logger.LogDebug($"{id} Started " +
                        $"| Url: {context.Request.GetDisplayUrl()} " +
                        $"| IP: {ip}");

        await next(context);

        stopwatch.Stop();
        logger.LogDebug($"{id} Finished in {stopwatch.Elapsed.TotalSeconds:0.####}s " +
                        $"| Url: {context.Request.GetDisplayUrl()} " +
                        $"| Response: {context.Response.StatusCode}");
    }
}