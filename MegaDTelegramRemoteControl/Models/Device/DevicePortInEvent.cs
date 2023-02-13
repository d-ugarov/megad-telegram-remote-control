using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortInEvent
{
    public required DeviceInPortCommand Command { get; set; }

    public int Counter { get; set; }

    public override string ToString()
    {
        return $"command: {Command.GetDescription()}, counter: {Counter}";
    }
}