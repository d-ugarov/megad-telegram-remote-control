using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Filters;
using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.Middlewares;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services;
using MegaDTelegramRemoteControl.Services.Interfaces;
using MegaDTelegramRemoteControl.Services.MegaDServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using SmartHome.Bot.Telegram;
using SmartHome.Common.AntiCaptcha;
using SmartHome.Common.Helpers;
using SmartHome.Common.Interfaces;
using SmartHome.Device.MegaD;
using SmartHome.Infrastructure.Helpers;
using SmartHome.PrivateOffice.Pes;
using SmartHome.Services;
using System;
using System.IO;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info($"[Main:{Constants.InstanceId}] Application started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseNLog();
    builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
    builder.ConfigureKestrel();
    AddSystemServices(builder);
    ConfigureServices(builder);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseResponseCaching();
    app.UseStaticFiles();
    app.UseRouting();
    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Fatal($"Application crashed: {e.Message}");
    logger.Error($"[Main:{Constants.InstanceId}] Application crashed: {e.Message} {Environment.NewLine}{e.StackTrace}");
    throw;
}
finally
{
    logger.Info($"[Main:{Constants.InstanceId}] Application stopped");
    LogManager.Shutdown();
}

void AddSystemServices(WebApplicationBuilder builder)
{
    builder.Services
           .AddMemoryCache()
           .AddResponseCaching()
           .AddMvcCore(x =>
           {
               x.Filters.Add(typeof(HttpResponseExceptionFilter));
           })
           .AddApiExplorer()
           .AddDataAnnotationsLocalization()
           .AddDataAnnotations()
           .AddJsonOptions(x =>
           {
               x.JsonSerializerOptions.AllowTrailingCommas = true;
               x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
           });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.ConfigureByType<PlatformConfig>();

    builder.Services.AddTransient<IBotHandler, BotHandler>();
    builder.Services.AddTransient<IMegaDEventsHandler, MegaDEventsHandler>();
    builder.Services.AddTransient<IHomeProcessor, HomeProcessor>();
    builder.Services.AddSingleton<IWarmupCacheService, InitService>();

    var homeConfig = CreateHomeConfig(builder);
    logger.Info(homeConfig.ToString());

    builder.Services.AddSingleton<IHomeService, HomeService>(x =>
        new HomeService(
            homeConfig,
            x.GetRequiredService<ILogger<HomeService>>()));

    builder.AddBotTelegram();
    builder.AddDeviceMegaD();

    builder.AddAntiCaptcha();
    builder.AddPrivateOfficePes();

    builder.ConfigureByType<JobSchedulerConfig>();
    builder.Services.AddHostedService<JobScheduler>();
}

HomeConfig CreateHomeConfig(WebApplicationBuilder builder)
{
    var automationConfig = builder.GetConfigByType<AutomationConfig>() ?? new();
    var deviceConfig = builder.GetConfigByType<DevicesConfig>() ?? new();
    var homeMapConfig = builder.GetConfigByType<HomeMapConfig>() ?? new();
    var keeneticProxyConfig = builder.GetConfigByType<KeeneticProxyConfig>() ?? new();

    return ConfigHelper.MakeConfig(deviceConfig, homeMapConfig, automationConfig, keeneticProxyConfig);
}