using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Serilog;
using Logging;
using IdentityService.Application;
using IdentityService.Infrastructure;
// 1. Убираем using Microsoft.IdentityModel.Logging;

namespace IdentityService.API;

public class Program
{
    public static void Main(string[] args)
    {
        // 2. Убираем отладочные флаги PII
        
        Console.WriteLine("fff World!");
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApiServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Host.UseSerilog((context, config) =>
        {
            SeriLogger.Configure(context, config);
        });
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSerilogRequestLogging();
        
        app.MapControllers();
        app.Run();
    }
}