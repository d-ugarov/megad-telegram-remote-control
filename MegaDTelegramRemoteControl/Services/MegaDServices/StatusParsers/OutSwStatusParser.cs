using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public class OutSwStatusParser : IStatusParser
{
    public DevicePortStatus Parse(DevicePort devicePort, string portStatus)
    {
        return new()
               {
                   InOutSwStatus = Enum.Parse<InOutSWStatus>(portStatus, true),
                   Port = devicePort,
               };
    }
}