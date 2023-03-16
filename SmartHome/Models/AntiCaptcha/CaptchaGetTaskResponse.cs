namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public record CaptchaGetTaskResponse : ICaptchaResponse
{
    public int ErrorId { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorDescription { get; init; }
    public CaptchaGetTaskResponseStatus Status { get; init; }
    public CaptchaSolution? Solution { get; init; }
    public decimal Cost { get; init; }
    public string Ip { get; init; } = "";
    public int CreateTime { get; init; }
    public int EndTime { get; init; }
    public int SolveCount { get; init; }
}

public enum CaptchaGetTaskResponseStatus
{
    Processing,
    Ready,
}