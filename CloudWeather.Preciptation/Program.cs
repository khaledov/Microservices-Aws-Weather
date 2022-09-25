using CloudWeather.Preciptation.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<PrecipDbContext>(opts =>
{
    opts.EnableSensitiveDataLogging();
    opts.EnableDetailedErrors();
    opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, 
ServiceLifetime.Transient
);


var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip,[FromQuery] int? days, PrecipDbContext db) =>
{
    if (days == null || days < 1 || days > 30)
    { 
        return Results.BadRequest("Please add a 'days' query parameter between 1 and 30"); 
    }
    var startDate = DateTime.UtcNow - TimeSpan.FromDays( days.Value);

    var result =await db.Precipitation
    .Where(precipt => precipt.ZipCode == zip && precipt.CreatedOn >= startDate)
    .ToListAsync();

    return Results.Ok(result);
    
});

app.MapPost("/observation", async (Precipitation precip, PrecipDbContext db) =>
{
    precip.CreatedOn = precip.CreatedOn.ToUniversalTime();
    await db.AddAsync<Precipitation>(precip);
    await db.SaveChangesAsync();
});

app.Run();
