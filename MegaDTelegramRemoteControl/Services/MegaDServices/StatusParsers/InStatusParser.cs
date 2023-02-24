using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System;

namespace MegaDTelegramRemoteControl.Services.MegaDServices.StatusParsers;

public class InStatusParser : IStatusParser
{
    public DevicePortInfo Parse(DevicePort devicePort, string portStatus)
    {
        var status = portStatus.Split('/');
        var counter = status.Length > 1 && int.TryParse(status[1], out var value)
            ? value
            : 0;

        return new()
               {
                   Port = devicePort,
                   Status = new()
                            {
                                InOutSwStatus = Enum.Parse<InOutSWStatus>(status[0], true),
                                InCounter = counter,
                            },
               };
    }
}