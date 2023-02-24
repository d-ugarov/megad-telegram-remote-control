using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public class OutSwStatusParser : IStatusParser
{
    public DevicePortInfo Parse(DevicePort devicePort, string portStatus)
    {
        return new()
               {
                   Port = devicePort,
                   Status = new()
                            {
                                InOutSwStatus = Enum.Parse<InOutSWStatus>(portStatus, true),
                            },
               };
    }
}