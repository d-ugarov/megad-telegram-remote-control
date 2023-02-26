using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Models.Interfaces;

public interface IConditions
{
    List<DevicePort> Ports { get; }
    ConditionType Type { get; init; }
    InOutSWStatus? Status { get; init; }
}