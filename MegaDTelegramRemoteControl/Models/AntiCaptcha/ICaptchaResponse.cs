namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public interface ICaptchaResponse
{
    int ErrorId { get; init; }
    string? ErrorCode { get; init; }
    string? ErrorDescription { get; init; }
}