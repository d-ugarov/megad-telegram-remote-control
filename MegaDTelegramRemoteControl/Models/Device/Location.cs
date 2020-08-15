using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class Location
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<LocationItems> Items { get; set; }
        public Location Parent { get; set; }
        public List<Location> SubLocations { get; set; }
    }
}