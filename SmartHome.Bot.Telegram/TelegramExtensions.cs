using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Bot.Telegram.Configurations;
using SmartHome.Common.Helpers;
using SmartHome.Common.Interfaces;

namespace SmartHome.Bot.Telegram;

public static class TelegramExtensions
{
    /// <summary>
    /// Inject <see cref="IBotService"/>
    /// </summary>
    public static void AddBotTelegram(this WebApplicationBuilder builder)
    {
        builder.ConfigureByType<TelegramConfig>();
        builder.Services.AddSingleton<IBotService, TelegramService>();
    }
}