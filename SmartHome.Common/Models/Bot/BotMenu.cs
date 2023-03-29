using System.Collections.Generic;

namespace SmartHome.Common.Models.Bot;

public record BotMenu
{
    public required string Text { get; init; }
    public List<BotMenuButton> Buttons { get; init; } = new();
    public List<BotMenuButton> FooterButtons { get; init; } = new();
}