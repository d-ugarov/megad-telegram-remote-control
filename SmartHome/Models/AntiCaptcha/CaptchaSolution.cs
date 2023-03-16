namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public record CaptchaSolution
{
    public string GRecaptchaResponse { get; init; } = "";
}