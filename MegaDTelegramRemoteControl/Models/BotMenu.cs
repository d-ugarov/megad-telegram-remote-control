﻿using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models;

public record BotMenu
{
    public required string Text { get; init; }
    public List<ButtonItem> Buttons { get; init; } = new();
}

public record ButtonItem
{
    public required string Name { get; set; }
    public required string Id { get; init; }
    public int Order { get; init; }
}