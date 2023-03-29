using SmartHome.Common.Helpers;
using SmartHome.PrivateOffice.Pes.Configurations;

namespace SmartHome.PrivateOffice.Pes;

public static class PesExtensions
{
    public static void AddPrivateOfficePes(this WebApplicationBuilder builder)
    {
        builder.ConfigureByType<PesConfig>();
    }
}