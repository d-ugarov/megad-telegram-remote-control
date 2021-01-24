using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Filters;
using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Services;
using MegaDTelegramRemoteControl.Infrastructure.Middlewares;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Services;
using MegaDTelegramRemoteControl.Services.Interfaces;
using MegaDTelegramRemoteControl.Services.MegaDServices;
using MegaDTelegramRemoteControl.Services.StubServices;
using MegaDTelegramRemoteControl.Services.TelegramServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System.Net;

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
                    .AddMvcCore(x =>
                    {
                        x.Filters.Add(typeof(HttpResponseExceptionFilter));
                    })
                    .AddApiExplorer()
                    .AddDataAnnotations()
                    .AddJsonOptions(x =>
                    {
                        x.JsonSerializerOptions.AllowTrailingCommas = true;
                        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });
            
            ConfigureCustomServices(services);
            ConfigureConfigs(services);
        }

        private void ConfigureConfigs(IServiceCollection services)
        {
            var deviceConfig = Configuration.GetSection(nameof(DevicesConfig)).Get<DevicesConfig>() ?? new();
            var homeMapConfig = Configuration.GetSection(nameof(HomeMapConfig)).Get<HomeMapConfig>() ?? new();
            var homeConfig = ConfigHelper.MakeConfig(deviceConfig, homeMapConfig);
            
            LogManager.GetCurrentClassLogger().Info(homeConfig.ToString());
            services.AddSingleton(_ => homeConfig);

            services.AddSingleton(_ => Configuration.GetSection(nameof(TelegramConfig)).Get<TelegramConfig>() ?? new());
            services.AddSingleton(_ => Configuration.GetSection(nameof(InternalSchedulerConfig)).Get<InternalSchedulerConfig>() ?? new());
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            var platformConfig = Configuration.GetSection(nameof(PlatformConfig)).Get<PlatformConfig>() ?? new();
            
            services.AddSingleton<IBotService, TelegramBotService>();
            services.AddTransient<IBotHandler, TelegramBotHandler>();
            services.AddTransient<IHomeLogic, HomeLogic>();
            services.AddSingleton<IHomeState, HomeState>();
            services.AddTransient<IDeviceCommandParser, MegaDCommandParser>();

            if (!platformConfig.UseFakeDeviceConnector)
                services.AddHttpClient<IDeviceConnector, MegaDConnector>();
            else
                services.AddTransient<IDeviceConnector, StubDeviceConnector>();
            
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
