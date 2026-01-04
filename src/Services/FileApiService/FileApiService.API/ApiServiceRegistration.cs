
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;

namespace FileApiService.API;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            
            options.AddSecurityDefinition("CookieAuth", new OpenApiSecurityScheme
            {
                Name = "token",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
            });
        });
        services.AddControllers();
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }
}
