namespace MegaDTelegramRemoteControl.Models.Device;

public class LocationItems
{
    public required string Id { get; init; }
    public required string? CustomName { get; init; }
    public required Device Device { get; init; }
    public required DevicePort Port { get; init; }
}