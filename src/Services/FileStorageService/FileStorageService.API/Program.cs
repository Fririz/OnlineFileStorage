using FileStorageService.Application;
using FileStorageService.Infrastructure;
using Serilog;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 5_368_709_120; // TODO bring it to env
});
builder.Host.UseSerilog((context, config) =>
{
    SeriLogger.Configure(context, config);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();
