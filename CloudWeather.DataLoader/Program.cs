
using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var servicesConfig = config.GetSection("Services");
var tempServiceConfig = servicesConfig.GetSection("Temperature");
var tempHost = tempServiceConfig["Host"];
var tempPort = tempServiceConfig["Port"];

var preciptServiceConfig = servicesConfig.GetSection("Precipitation");
var preciptHost = preciptServiceConfig["Host"];
var preciptPort = preciptServiceConfig["Port"];

var zipCodes= new List<string>
{
    "16445",
    "12740",
    "10316",
    "10465",
    "11120",
    "11121"
};
Console.WriteLine("Starting Data Loading ...");

var temperatureClient = new HttpClient();
temperatureClient.BaseAddress = new Uri($"http://{tempHost}:{tempPort}");

var precipClient=new HttpClient();
precipClient.BaseAddress = new Uri($"http://{preciptHost}:{preciptPort}");

foreach(var code in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {code}");
    var from = DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;
    for(var day= from.Date; day.Date<= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemp(code, day, temperatureClient);
        PostPrecip(temps[0], code, day, precipClient);
    }
}

async Task PostPrecip(int lowTemp, string zip, DateTime day, HttpClient precipClient)
{
    var rand=new Random();
    var isPrecip = rand.Next(2) < 1;
    PrecipitationModel precipitation ;
    if (isPrecip)
    {
        var precipInches = rand.Next(1, 16);
        if (lowTemp < 32)
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "snow",
                ZipCode = zip,
                CreatedOn = day,
            };
        }
        else
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "rain",
                ZipCode = zip,
                CreatedOn = day,
            };
        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountInches = 0,
            WeatherType = "none",
            ZipCode = zip,
            CreatedOn = day,
        };
    }
    var preciptResponse = precipClient
        .PostAsJsonAsync("observation", precipitation)
        .Result;

    if (preciptResponse.IsSuccessStatusCode)
    {
        Console.Write($"Posted Precipitation: Date: {day:d}"+
            $"Zip: {zip}" +
            $"Type: {precipitation.WeatherType}"+
            $"Amount (in.): {precipitation.AmountInches}");
    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient temperatureClient)
{
   var rand=new Random();
    var t1 = rand.Next(0, 100);
    var t2= rand.Next(0, 100);
    var hiloTemps = new List<int> { t1, t2 };
    hiloTemps.Sort();

    var tempObservation = new TemperatureModel
    {
        TempLow = hiloTemps[0],
        TempHigh = hiloTemps[1],
        ZipCode = zip,
        CreatedOn = day
    };

    var tempResponse = temperatureClient
        .PostAsJsonAsync("observation", tempObservation)
        .Result;

    if (tempResponse.IsSuccessStatusCode)
    {
        Console.Write($"Posted Temperature: Date: {day:d}" +
            $"Zip: {zip}" +
            $"Lo (C):{hiloTemps[0]}" +
            $"Hi (C): {hiloTemps[1]}");
    }
    else
    {
        Console.WriteLine(tempResponse.ToString()); 
    }
    return hiloTemps;
}