using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Location
    {
        public string Name { get; init; } = null!;
        public string Id { get; init; } = null!;
        public List<LocationItems> Items { get; init; } = new();
        public Location? Parent { get; init; }
        public List<Location> SubLocations { get; init; } = new();
    }
}