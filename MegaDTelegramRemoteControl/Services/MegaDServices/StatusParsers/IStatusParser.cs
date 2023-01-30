using MegaDTelegramRemoteControl.Models.Device;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public interface IStatusParser
{
    DevicePortStatus Parse(DevicePort devicePort, string portStatus);
}