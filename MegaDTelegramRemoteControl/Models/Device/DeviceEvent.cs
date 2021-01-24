using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Text;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DeviceEvent
    {
        private DevicePortEvent? port;
        
        public DeviceEventType Type { get; set; }
        
        public bool IsParsedSuccessfully { get; set; }

        public Device? Device { get; init; }

        public DevicePortEvent? Event
        {
            get => Type == DeviceEventType.PortEvent
                ? port
                : throw new OperationException($"Port is not available for event type {Type}");
            set => port = value;
        }
        
        public static DeviceEvent UnknownEvent => new() {Type = DeviceEventType.Unknown};
        
        public override string ToString()
        {
            var str = new StringBuilder($"Event: {Type}");

            if (Device != null)
                str.Append($", device: {Device.Name}");

            if (port?.Port != null)
                str.Append($", {port}");

            str.Append(IsParsedSuccessfully ? " (ok)" : " (error)");

            return str.ToString();
        }
    }
}