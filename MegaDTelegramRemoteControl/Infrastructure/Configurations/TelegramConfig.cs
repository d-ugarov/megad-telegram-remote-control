using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record TelegramConfig
{
    public string BotAccessToken { get; init; } = null!;

    public HashSet<int> AllowedUsers { get; init; } = new();
}