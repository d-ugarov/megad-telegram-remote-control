namespace SmartHome.Common.Models.AntiCaptcha;

public record AntiCaptchaRequest(AntiCaptchaType Type, AntiCaptchaReCaptchaV2? ReCaptchaV2);

public record AntiCaptchaReCaptchaV2(string WebsiteUrl, string WebsiteKey, bool IsInvisible);

public enum AntiCaptchaType
{
    ReCaptchaV2,
}