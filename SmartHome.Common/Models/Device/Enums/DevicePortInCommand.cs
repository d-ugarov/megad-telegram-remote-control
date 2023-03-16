namespace SmartHome.Common.Models.Device.Enums;

public enum DevicePortInCommand
{
    /// <summary>
    /// Click (default)
    /// </summary>
    KeyPressed,

    /// <summary>
    /// Click and release
    /// </summary>
    KeyReleased,

    /// <summary>
    /// Single click 
    /// </summary>
    Click,

    /// <summary>
    /// Double click
    /// </summary>
    DoubleClick,

    /// <summary>
    /// Long click
    /// </summary>
    LongClick,
}