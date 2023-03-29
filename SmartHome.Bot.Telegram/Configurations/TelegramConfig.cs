using System.Collections.Generic;

namespace SmartHome.Bot.Telegram.Configurations;

internal record TelegramConfig
{
    public string BotAccessToken { get; init; } = null!;

    public HashSet<long> AllowedUsers { get; init; } = new();
}