using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public class HomeMapConfig
    {
        public List<Location> Locations { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        
        public List<LocationItems> Items { get; set; }
        
        public List<Location> SubLocations { get; set; }
    }

    public class LocationItems
    {
        public string DeviceId { get; set; }
        public string PortId { get; set; }
    }
}