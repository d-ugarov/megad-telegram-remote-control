using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public class SwStatusParser : IStatusParser
{
    public DevicePortStatus Parse(DevicePort devicePort, string portStatus)
    {
        return new()
               {
                   SWStatus = Enum.Parse<SWStatus>(portStatus, true),
                   Port = devicePort,
               };
    }
}