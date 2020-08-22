using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Services;
using MegaDTelegramRemoteControl.Infrastructure.Middlewares;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Infrastructure.Services;
using MegaDTelegramRemoteControl.Services;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MegaDTelegramRemoteControl
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(x =>
            {
                var kestrelConfig = Configuration.GetSection(nameof(Kestrel)).Get<Kestrel>();

                if (!string.IsNullOrEmpty(kestrelConfig?.Url))
                    x.Listen(IPAddress.Parse(kestrelConfig.Url), kestrelConfig.Port);
            });
            
            services.AddMemoryCache()
                    .AddResponseCaching()
                    .AddMvcCore()
                    .AddApiExplorer()
                    .AddDataAnnotations()
                    .AddJsonOptions(x =>
                    {
                        x.JsonSerializerOptions.AllowTrailingCommas = true;
                        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });
            
            ConfigureCustomServices(services);
            ConfigureConstants(services);
        }

        private void ConfigureConstants(IServiceCollection services)
        {
            var deviceConfig = Configuration.GetSection(nameof(DevicesConfig)).Get<DevicesConfig>();
            var homeMapConfig = Configuration.GetSection(nameof(HomeMapConfig)).Get<HomeMapConfig>();
            var homeConfig = ConfigHelper.MakeConfig(deviceConfig, homeMapConfig);
            
            var logger = services.BuildServiceProvider().GetRequiredService<StartupLogger>();
            logger.Log(homeConfig.ToString());
            services.AddSingleton(_ => homeConfig);
            
            services.AddSingleton(_ => Configuration.GetSection(nameof(TelegramConfig)).Get<TelegramConfig>());
            services.AddSingleton(_ => Configuration.GetSection(nameof(InternalSchedulerConfig)).Get<InternalSchedulerConfig>());
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddSingleton(x => new StartupLogger(x.GetRequiredService<ILogger<StartupLogger>>()));
            
            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddTransient<IDeviceEventParser, DeviceEventParser>();
            services.AddTransient<IDevicePortStatusParser, DevicePortStatusParser>();
            services.AddTransient<IHomeLogic, HomeLogic>();
            services.AddTransient<ITelegramLogic, TelegramLogic>();
            services.AddHttpClient<IDeviceConnector, DeviceConnector>();
            
            services.AddHostedService<InitService>();
            services.AddHostedService<JobScheduler>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<RequestLoggingMiddleware>();
            
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
