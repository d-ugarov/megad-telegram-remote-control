using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHome.Common.Infrastructure.Helpers;

public static class PlatformJsonSerialization
{
    public static readonly JsonSerializerOptions Options = new()
                                                           {
                                                               PropertyNameCaseInsensitive = true,
                                                               NumberHandling = JsonNumberHandling.AllowReadingFromString,
                                                               Converters =
                                                               {
                                                                   new JsonStringEnumConverter(),
                                                               }
                                                           };
}