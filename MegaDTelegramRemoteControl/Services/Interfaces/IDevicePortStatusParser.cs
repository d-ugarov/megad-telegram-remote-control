using MegaDTelegramRemoteControl.Models.Device;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IDevicePortStatusParser
    {
        DevicePortStatus ParseStatus(DevicePort port, string portStatus);
    }
}