using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace IdentityService.API;

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
        
        
        var validIssuer = configuration["Jwt:Issuer"] 
            ?? throw new InvalidOperationException("Jwt:Issuer is not set");
        var validAudience = configuration["Jwt:Audience"] 
            ?? throw new InvalidOperationException("Jwt:Audience is not set");
        var jwtKey = configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("Jwt:Key is not set");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MyCookieAuth";
            options.DefaultChallengeScheme = "MyCookieAuth";
            options.DefaultScheme = "MyCookieAuth"; 
        })
            .AddJwtBearer("MyCookieAuth", options =>
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

        return services;
    }
}
