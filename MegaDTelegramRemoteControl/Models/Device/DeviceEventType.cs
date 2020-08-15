namespace MegaDTelegramRemoteControl.Models.Device
{
    public enum DeviceEventType
    {
        [DeviceEventAttribute("")]
        Unknown,
        
        [DeviceEventAttribute("st")]
        DeviceStarted,
        
        [DeviceEventAttribute("pt")]
        PortEvent,
    }
}