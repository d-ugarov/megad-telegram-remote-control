using SmartHome.Common.Interfaces;

namespace SmartHome.Device.MegaD;

public static class MegaDExtensions
{
    /// <summary>
    /// Inject <see cref="IDeviceConnector"/> and <see cref="IDeviceCommandParser"/>
    /// </summary>
    public static void AddDeviceMegaD(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IDeviceConnector, MegaDConnector>();
        builder.Services.AddTransient<IDeviceCommandParser, MegaDCommandParser>();
    }
}