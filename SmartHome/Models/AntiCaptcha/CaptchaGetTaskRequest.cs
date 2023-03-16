using System.Text.Json.Serialization;

namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public record CaptchaGetTaskRequest : ICaptchaRequest
{
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; init; } = "";
    [JsonPropertyName("taskId")]
    public int TaskId { get; init; }
}