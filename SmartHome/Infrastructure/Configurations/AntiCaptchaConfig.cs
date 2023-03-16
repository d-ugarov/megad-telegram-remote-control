namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public class AntiCaptchaConfig
{
    public string ApiKey { get; set; } = "";
    public int MaxSolveDurationInSeconds { get; set; } = 30;
}