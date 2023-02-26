using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Models.Interfaces;

public interface IDestinationRule
{
    DevicePort Port { get; init; }
    TriggerRuleAction Action { get; init; }
}