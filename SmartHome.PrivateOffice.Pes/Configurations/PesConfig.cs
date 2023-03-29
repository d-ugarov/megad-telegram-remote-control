namespace SmartHome.PrivateOffice.Pes.Configurations;

public class PesConfig
{
    public List<PesClientConfig> Clients { get; set; } = new();
    public PesCommonConfig CommonConfig { get; set; } = new();
}

public class PesClientConfig
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class PesCommonConfig
{
    public string? CaptchaWebsiteKey { get; set; }
}