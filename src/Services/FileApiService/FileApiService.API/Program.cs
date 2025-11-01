using FileApiService.API;
using FileApiService.Application;
using FileApiService.Infrastructure;
using Serilog;
using Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();