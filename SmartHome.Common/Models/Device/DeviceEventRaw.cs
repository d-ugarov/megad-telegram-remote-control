using System.Collections.Generic;

namespace SmartHome.Common.Models.Device;

public record DeviceEventRaw(IReadOnlyCollection<DeviceEventRawItem> Items);

public record DeviceEventRawItem(string Key, string Value);