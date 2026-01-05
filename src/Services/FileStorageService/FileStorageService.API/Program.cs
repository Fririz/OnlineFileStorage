using System.Reflection;
using FileStorageService.Application;
using FileStorageService.Infrastructure;
using Serilog;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();
