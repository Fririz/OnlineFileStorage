using FileApiService.API;
using FileApiService.Application;
using FileApiService.Infrastructure;
using FileApiService.Infrastructure.Persistence;
using Serilog;
using Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Host.UseSerilog((context, config) =>
{
    SeriLogger.Configure(context, config);
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try 
    {
        var context = services.GetRequiredService<Context>();
                
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate(); 
            logger.LogInformation("Migration success");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();