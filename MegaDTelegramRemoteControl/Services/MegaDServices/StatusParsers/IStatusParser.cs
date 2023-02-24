using MegaDTelegramRemoteControl.Models.Device;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public interface IStatusParser
{
    DevicePortInfo Parse(DevicePort devicePort, string portStatus);
}