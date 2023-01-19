using MegaDTelegramRemoteControl.Models.Device.Enums;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortAction
{
    public DeviceOutPortCommand? SWCommand { get; private init; }

    public static DevicePortAction CommandSWDefault => new() {SWCommand = DeviceOutPortCommand.Switch};
    public static DevicePortAction CommandSW(DeviceOutPortCommand command) => new() {SWCommand = command};
}