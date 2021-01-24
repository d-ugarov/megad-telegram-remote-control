namespace MegaDTelegramRemoteControl.Models.Device
{
    public class LocationItems
    {
        public string Id { get; init; } = null!;
        public string? CustomName { get; init; }
        public Device Device { get; init; } = null!;
        public DevicePort Port { get; init; } = null!;
    }
}