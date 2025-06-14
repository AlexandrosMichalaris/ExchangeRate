using ExchangeRate.Application.Service;
using ExchangeRate.Configuration.DI;
using ExchangeRate.Infrastructure.Configuration;
using ExchangeRate.Infrastructure.Configuration.Database;
using ExchangeRate.Middleware;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EcbEuropaSettings>(builder.Configuration.GetSection("ApiSettings:EcbEuropa"));

// Configure services using the static class method
builder.Services.ConfigureServices();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

builder.Services.AddQuartzHostedService();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Replace default logging with Serilog and Read Serilog config from appsettings.json
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // Register the exception middleware

app.Run();
