using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Serilog;
using Logging;
using IdentityService.Application;
using IdentityService.Infrastructure;


namespace IdentityService.API;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("1111 World!");
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApiServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Host.UseSerilog((context, config) =>
        {
            SeriLogger.Configure(context, config);
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.MapControllers();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseSerilogRequestLogging();
        app.Run();
    }
}