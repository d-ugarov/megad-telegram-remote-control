namespace MegaDTelegramRemoteControl.Models.Device.Enums
{
    public enum DeviceEventType
    {
        [DeviceEvent("")]
        Unknown,
        
        [DeviceEvent("st")]
        DeviceStarted,
        
        [DeviceEvent("pt")]
        PortEvent,
    }
}