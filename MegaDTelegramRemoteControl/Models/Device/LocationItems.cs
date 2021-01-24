namespace MegaDTelegramRemoteControl.Models.Device
{
    public class LocationItems
    {
        public string Id { get; set; } = null!;
        public string? CustomName { get; set; }
        public Device Device { get; set; } = null!;
        public DevicePort Port { get; set; } = null!;
    }
}