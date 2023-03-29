using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SmartHome.Common.Helpers;

public static class WebApplicationExtensions
{
    public static void ConfigureByType<T>(this WebApplicationBuilder webAppBuilder) where T : class, new()
    {
        webAppBuilder.Services.Configure<T>(webAppBuilder.GetSectionByType<T>());
    }

    public static void ConfigureByType<T>(this WebApplicationBuilder webAppBuilder, T option) where T : class, new()
    {
        webAppBuilder.Services.Configure<T>(_ => Options.Create(option));
    }

    public static void ConfigureByTypeAsSingleton<T>(this WebApplicationBuilder webAppBuilder) where T : class, new()
    {
        webAppBuilder.Services.AddSingleton(_ => webAppBuilder.GetConfigByType<T>() ?? new());
    }

    public static T? GetConfigByType<T>(this WebApplicationBuilder webAppBuilder) where T : class, new()
    {
        return webAppBuilder.GetSectionByType<T>().Get<T>();
    }

    public static IConfigurationSection GetSectionByType<T>(this WebApplicationBuilder webAppBuilder) where T : class, new()
    {
        return webAppBuilder.Configuration.GetSection(typeof(T).Name);
    }
}