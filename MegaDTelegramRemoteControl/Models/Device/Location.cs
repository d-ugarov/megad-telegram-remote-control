using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Location
    {
        public string Name { get; set; } = null!;
        public string Id { get; set; } = null!;
        public List<LocationItems> Items { get; set; } = new();
        public Location? Parent { get; set; }
        public List<Location> SubLocations { get; set; } = new();
    }
}