using MegaDTelegramRemoteControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Models
{
    public class HomeConfig
    {
        public Dictionary<string, Device.Device> Devices { get; set; }
        public List<Location> Locations { get; set; }

        public override string ToString()
        {
            static IEnumerable<string> LocationsLog(IEnumerable<Location> locations) =>
                locations.Select(x =>
                    $"'{x.Name}'. " +
                    $"Items: {(x.Items.Any() ? string.Join(", ", x.Items.Select(p => p.Port.Name)) : "-")}, " +
                    $"Locations: {(x.SubLocations.Any() ? string.Join(", ", LocationsLog(x.SubLocations)) : "-")}");

            var devices = string.Join(Environment.NewLine,
                Devices.Select(x =>
                    $"{x.Key}: {x.Value.Name}, {x.Value.Ip}, " +
                    $"ports: {string.Join(", ", x.Value.Ports.Select(p => $"{p.Key} {p.Value.Type} '{p.Value.Name}'"))}"));
            var locations = string.Join(Environment.NewLine, LocationsLog(Locations));

            return $"HomeConfig:{Environment.NewLine}" +
                   $"Devices:{Environment.NewLine}" +
                   $"{devices}{Environment.NewLine}" +
                   $"Locations:{Environment.NewLine}" +
                   $"{locations}";
        }
    }
}