using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models;

public record BotMenu
{
    public required string Text { get; init; }
    public List<ButtonItem> Buttons { get; init; } = new();
    public List<ButtonItem> FooterButtons { get; init; } = new();
}

public record ButtonItem
{
    public required string Name { get; init; }
    public required string ActionId { get; init; }
    public int Order { get; init; }
}