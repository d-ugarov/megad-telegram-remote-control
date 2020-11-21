using MegaDTelegramRemoteControl.Models.Device;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface IHomeState
    {
        void Set(DeviceEvent deviceEvent);

        DevicePortEvent Get(DevicePort port);
        
        void Clean();
    }
}