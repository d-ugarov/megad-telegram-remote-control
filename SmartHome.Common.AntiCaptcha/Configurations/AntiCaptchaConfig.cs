namespace SmartHome.Common.AntiCaptcha.Configurations;

internal class AntiCaptchaConfig
{
    public string ApiKey { get; set; } = "";
    public int MaxSolveDurationInSeconds { get; set; } = 30;
}