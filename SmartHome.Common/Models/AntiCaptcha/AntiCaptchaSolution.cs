namespace SmartHome.Common.Models.AntiCaptcha;

public record AntiCaptchaSolution(AntiCaptchaType Type, AntiCaptchaReCaptchaV2Solution? ReCaptchaV2);

public record AntiCaptchaReCaptchaV2Solution(string GRecaptchaResponse);