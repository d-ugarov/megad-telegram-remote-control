namespace SmartHome.Common.AntiCaptcha.Models;

internal record CaptchaSolution
{
    public string GRecaptchaResponse { get; init; } = "";
}