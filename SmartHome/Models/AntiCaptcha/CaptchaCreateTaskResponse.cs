namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public record CaptchaCreateTaskResponse : ICaptchaResponse
{
    public int ErrorId { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorDescription { get; init; }
    public int TaskId { get; init; }
}