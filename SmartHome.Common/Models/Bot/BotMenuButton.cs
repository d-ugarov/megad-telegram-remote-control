namespace SmartHome.Common.Models.Bot;

public record BotMenuButton
{
    public required string Name { get; init; }
    public required string ActionId { get; init; }
    public int Order { get; init; }
}