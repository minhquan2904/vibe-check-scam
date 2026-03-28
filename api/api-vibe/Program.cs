using api_vibe.HealthCheck;
using api_vibe.HealthCheck.Checks;
using api_vibe.Options;
using api_vibe.Infrastructure.Gemini;
using api_vibe.Services;
using api_vibe.Services.Impls;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Health Check
builder.Services.Configure<HealthCheckOptions>(
    builder.Configuration.GetSection("HealthCheck"));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDependencyHealthCheck, DatabaseHealthCheck>();
builder.Services.AddScoped<IDependencyHealthCheck, CacheHealthCheck>();
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

// Gemini & Gas Price
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<GasPriceOptions>(
    builder.Configuration.GetSection("GasPrice"));

builder.Services.AddHttpClient<IGeminiClient, GeminiClient>();
builder.Services.AddScoped<IGasPriceService, GasPriceService>();
builder.Services.AddScoped<IStatementProcessingService, StatementProcessingService>();
builder.Services.AddScoped<ICheckScamService, CheckScamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

