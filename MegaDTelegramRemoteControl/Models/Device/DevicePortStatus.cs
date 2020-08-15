namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DevicePortStatus
    {
        public SimpleStatus SimpleStatus { get; set; }
    }

    public enum SimpleStatus
    {
        On, 
        Off,
    }
}