using System.Text.Json.Serialization;

namespace SmartHome.Common.AntiCaptcha.Models;

internal record CaptchaGetTaskRequest : ICaptchaRequest
{
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; init; } = "";
    [JsonPropertyName("taskId")]
    public int TaskId { get; init; }
}