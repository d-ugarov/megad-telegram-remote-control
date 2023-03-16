using System.Text.Json.Serialization;

namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public record CaptchaCreateTaskRequest : ICaptchaRequest
{
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; init; } = "";
    [JsonPropertyName("task")]
    public CaptchaTask Task { get; init; } = null!;
}

public interface ICaptchaTask : IReCaptchaV2Task
{
    string Type { get; init; }
}

public interface IReCaptchaV2Task
{
    string WebsiteUrl { get; init; }
    string WebsiteKey { get; init; }
    bool IsInvisible { get; init; }
}

public record CaptchaTask : ICaptchaTask
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

public record ReCaptchaV2Task(string WebsiteUrl, string WebsiteKey, bool IsInvisible) : IReCaptchaV2Task;