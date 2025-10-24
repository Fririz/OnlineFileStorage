using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.API;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // 1. Возвращаем чтение из IConfiguration
        var validIssuer = configuration["Jwt:Issuer"] 
            ?? throw new InvalidOperationException("Jwt:Issuer is not set");
        var validAudience = configuration["Jwt:Audience"] 
            ?? throw new InvalidOperationException("Jwt:Audience is not set");
        var jwtKey = configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("Jwt:Key is not set");

        // 2. Оставляем рабочую конфигурацию схемы по умолчанию
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MyCookieAuth";
            options.DefaultChallengeScheme = "MyCookieAuth";
            options.DefaultScheme = "MyCookieAuth"; 
        })
            .AddJwtBearer("MyCookieAuth", options => // 3. Настраиваем нашу именованную схему
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer,

                    ValidateAudience = true,
                    ValidAudience = validAudience,

                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                // 4. Оставляем чистый обработчик OnMessageReceived
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["token"];
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
