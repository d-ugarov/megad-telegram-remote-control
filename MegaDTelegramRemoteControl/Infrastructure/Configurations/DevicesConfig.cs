using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public class DevicesConfig
    {
        public List<Device> Devices { get; set; }
    }
    
    /// <summary> MegaD </summary>
    public class Device
    {
        /// <summary>
        /// Device id
        /// <para>Used to distinguish events from multiple MegaDs</para>
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        
        public List<DevicePort> DevicePorts { get; set; }
    }

    public class DevicePort
    {
        public string Id { get; set; }
        public DevicePortType Type { get; set; }
        public string Name { get; set; }
        public DeviceOutPortMode? OutMode { get; set; }
        public Dictionary<string, string> OutModeIcons { get; set; }
    }

    public enum DevicePortType
    {
        IN,
        OUT,
        DSen,
        I2C,
        ADC,
    }

    public enum DeviceInPortMode
    {
        P,
        PR,
        R,
        C,
    }

    public enum DeviceOutPortMode
    {
        SW,
        PWM,
        SWLink,
        DS2413,
    }
}