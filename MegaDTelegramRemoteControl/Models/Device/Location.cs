using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.Device;

public record Location
{
    public required string Name { get; init; }
    public required string Id { get; init; }
    public required Location? Parent { get; init; }
    public List<LocationItems> Items { get; } = new();
    public List<LocationCondition> ItemsConditions { get; } = new();
    public List<Location> SubLocations { get; } = new();
}

public record LocationItems
{
    public required string ActionId { get; init; }
    public required Device Device { get; init; }
    public required DevicePort Port { get; init; }
    public string? CustomName { get; init; }

    public string FormattedName => CustomName ?? Port.Name;
}

public class LocationCondition
{
    public required LocationConditionType Type { get; init; }
    public required InOutSWStatus? Status { get; init; }
    public List<LocationItems> Items { get; } = new();
}

public enum LocationConditionType
{
    PortOutCurrentStatus,
}