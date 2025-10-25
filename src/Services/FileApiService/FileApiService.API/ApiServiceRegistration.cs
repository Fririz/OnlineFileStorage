
using System.Text;

namespace FileApiService.API;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddControllers();
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }
}
