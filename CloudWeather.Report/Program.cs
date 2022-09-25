using CloudWeather.Report.BusinessLogic;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services
    .Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

builder.Services.AddDbContext<WeatherReportDbContext>(opts =>
{
    opts.EnableSensitiveDataLogging();
    opts.EnableDetailedErrors();
    opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
},
ServiceLifetime.Transient
);

var app = builder.Build();

app.MapGet("/weather-report/{zip}", 
    async (string zip,
    [FromQuery] int? days,
    IWeatherReportAggregator aggregator) => {
        if (days == null || days < 1 || days > 30)
        {
            return Results.BadRequest("Please add a 'days' query parameter between 1 and 30");
        }
        var report= await aggregator.BuildReport(zip, days.Value);
        return Results.Ok(report);
    });

app.Run();
