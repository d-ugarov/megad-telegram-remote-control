using System.Text.Json.Serialization;

namespace MegaDTelegramRemoteControl.Models.PES;

public record PesAuthResult
{
    [JsonPropertyName("access_token")]
    public string Token { get; init; } = "";
}