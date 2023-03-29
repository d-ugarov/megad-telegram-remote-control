using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace SmartHome.Infrastructure.Helpers;

public static class WebApplicationSExtensions
{
    public static void ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KestrelServerOptions>(x =>
        {
            var kestrel = builder.Configuration.GetSection(nameof(KestrelConfig)).Get<KestrelConfig>();

            switch (kestrel)
            {
                case {Url: not null or "", Port: > 0}:
                    x.Listen(IPAddress.Parse(kestrel.Url), kestrel.Port);
                    break;
                case {Port: > 0}:
                    x.Listen(IPAddress.Any, kestrel.Port);
                    break;
            }
        });
    }
}