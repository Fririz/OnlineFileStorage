using Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace OcelotApiGateway;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot();
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 5_368_709_120; // TODO bring it to env
        });
        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",
                    "http://127.0.0.1:5173",
                    "http://192.168.1.105:5173"
                )
                .AllowAnyMethod()   
                .AllowAnyHeader()
                .AllowCredentials();
        }));

        builder.Host.UseSerilog((context, config) =>
        {
            config.MinimumLevel.Debug(); 
            SeriLogger.Configure(context, config);
        });
    
        var validIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not set");
        var validAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not set");
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not set");

        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options => 
            {
                options.RequireHttpsMetadata = false; 
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
                
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("token"))
                        {
                            context.Token = context.Request.Cookies["token"];
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var principal = context.Principal;
                        if (principal is not null)
                        {
                            var nickname = principal.FindFirst(ClaimTypes.Name)?.Value;
                            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!string.IsNullOrWhiteSpace(nickname))
                            {
                                context.Request.Headers["Nickname"] = nickname;
                            }
                            if (!string.IsNullOrWhiteSpace(id))
                            {
                                context.Request.Headers["Id"] = id;
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        var app = builder.Build();

        app.UseRouting();
        
        app.UseCors("MyPolicy"); 
        
        app.UseAuthentication();
        app.UseAuthorization();

        await app.UseOcelot();
        await app.RunAsync();
    }
}

