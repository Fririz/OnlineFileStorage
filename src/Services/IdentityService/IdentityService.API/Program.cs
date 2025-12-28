using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Serilog;
using Logging;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API;

public class Program
{
    public static void Main(string[] args)
    {
        
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
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try 
            {
                var context = services.GetRequiredService<UserContext>();
                
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate(); 
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
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