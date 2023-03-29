using System.Text.Json.Serialization;

namespace SmartHome.Common.AntiCaptcha.Models;

internal record CaptchaCreateTaskRequest : ICaptchaRequest
{
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; init; } = "";
    [JsonPropertyName("task")]
    public CaptchaTask Task { get; init; } = null!;
}

internal interface ICaptchaTask : IReCaptchaV2Task
{
    string Type { get; init; }
}

internal interface IReCaptchaV2Task
{
    string WebsiteUrl { get; init; }
    string WebsiteKey { get; init; }
    bool IsInvisible { get; init; }
}

internal record CaptchaTask : ICaptchaTask
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "";
    [JsonPropertyName("websiteURL")]
    public string WebsiteUrl { get; init; } = "";
    [JsonPropertyName("websiteKey")]
    public string WebsiteKey { get; init; } = "";
    [JsonPropertyName("isInvisible")]
    public bool IsInvisible { get; init; } 
}