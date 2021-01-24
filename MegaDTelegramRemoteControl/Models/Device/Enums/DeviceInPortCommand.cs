using System.ComponentModel;

namespace MegaDTelegramRemoteControl.Models.Device.Enums
{
    public enum DeviceInPortCommand
    {
        /// <summary>
        /// Click (default)
        /// </summary>
        [Description("Button pressed")]
        KeyPressed,
        
        /// <summary>
        /// Click and release
        /// </summary>
        [Description("Button released")]
        KeyReleased,
        
        /// <summary>
        /// Single click 
        /// </summary>
        [Description("Button pressed")]
        Click,
        
        /// <summary>
        /// Double click
        /// </summary>
        [Description("Double press")]
        DoubleClick,
        
        /// <summary>
        /// Long click
        /// </summary>
        [Description("Long press")]
        LongClick,
    }
}