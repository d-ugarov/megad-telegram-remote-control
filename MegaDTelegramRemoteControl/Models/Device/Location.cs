using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public class Location
{
    public required string Name { get; init; }
    public required string Id { get; init; }
    public required Location? Parent { get; init; }
    public List<LocationItems> Items { get; } = new();
    public List<Location> SubLocations { get; } = new();
}