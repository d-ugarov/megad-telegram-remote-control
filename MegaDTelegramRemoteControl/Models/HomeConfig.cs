using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaDTelegramRemoteControl.Models
{
    public class HomeConfig
    {
        public Dictionary<string, Device.Device> Devices { get; set; }
        public List<Location> Locations { get; set; }
        
        public override string ToString()
        {
            var log = new StringBuilder();
            
            void LogLocations(Location tempLocation, int offset = 0)
            {
                log.AppendLine($"{Extensions.GetWhitespaces(offset + 2)}" +
                               $"[{tempLocation.Name}]");

                log.AppendLine($"{Extensions.GetWhitespaces(offset + 3)}" +
                               $"Items: {(tempLocation.Items.Any() ? "" : "-")}");

                foreach (var item in tempLocation.Items)
                {
                    log.AppendLine($"{Extensions.GetWhitespaces(offset + 4)}" +
                                   $"[{item.CustomName ?? item.Port.Name}] Device {item.Device.Name}, Port {item.Port.Id}");
                }

                log.AppendLine($"{Extensions.GetWhitespaces(offset + 3)}" +
                               $"SubLocations: {(tempLocation.SubLocations.Any() ? "" : "-")}");

                foreach (var subLocation in tempLocation.SubLocations)
                {
                    LogLocations(subLocation, offset + 2);
                }
            }
            
            log.AppendLine("HomeConfig:");

            log.AppendLine($"{Extensions.GetWhitespaces(1)}Devices:");
            foreach (var (deviceKey, device) in Devices)
            {
                log.AppendLine($"{Extensions.GetWhitespaces(2)}" +
                               $"[{deviceKey}] {device.Name}, {device.Ip}, ports: {(device.Ports.Any() ? "" : "-")}");

                foreach (var (portKey, port) in device.Ports)
                {
                    log.AppendLine($"{Extensions.GetWhitespaces(3)}" +
                                   $"[{portKey}] {port.Type} '{port.Name}'");
                }
            }

            log.AppendLine($"{Extensions.GetWhitespaces(1)}Locations:");
            foreach (var location in Locations)
            {
                LogLocations(location);
            }
            
            return log.ToString();
        }
    }
}