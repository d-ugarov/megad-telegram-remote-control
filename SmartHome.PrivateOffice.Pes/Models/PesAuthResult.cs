using System.Text.Json.Serialization;

namespace SmartHome.PrivateOffice.Pes.Models;

internal record PesAuthResult
{
    [JsonPropertyName("access_token")]
    public string Token { get; init; } = "";
}