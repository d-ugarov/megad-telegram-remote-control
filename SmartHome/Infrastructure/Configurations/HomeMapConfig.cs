using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record HomeMapConfig
{
    public List<Location> Locations { get; init; } = new();
}

public record Location
{
    public string Name { get; init; } = null!;
    public List<LocationItems> Items { get; init; } = new();
    public List<LocationCondition> ItemsConditions { get; init; } = new();
    public List<Location> SubLocations { get; init; } = new();
}

public record LocationItems
{
    public string DeviceId { get; init; } = null!;
    public int PortId { get; init; }
    public string? CustomName { get; init; }
}

public class LocationCondition
{
    public required LocationConditionType Type { get; set; }
    public required InOutSWStatus? Status { get; set; }
}