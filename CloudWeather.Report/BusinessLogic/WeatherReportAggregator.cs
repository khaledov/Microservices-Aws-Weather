using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CloudWeather.Report.BusinessLogic
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherReportAggregator : IWeatherReportAggregator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherReportAggregator> _logger;
        private readonly WeatherDataConfig _weatherDataConfig;
        private readonly WeatherReportDbContext _weatherReportDbContext;

        public WeatherReportAggregator(IHttpClientFactory httpClientFactory,
            ILogger<WeatherReportAggregator> logger,
            IOptions<WeatherDataConfig> weatherDataConfig,
            WeatherReportDbContext weatherReportDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _weatherDataConfig = weatherDataConfig.Value;
            _weatherReportDbContext = weatherReportDbContext;
        }

        public async Task<WeatherReport> BuildReport(string zip, int days)
        {
            var httpclient = _httpClientFactory.CreateClient();
            var precipData = await FetchPrecipitationData(httpclient, zip, days);
            var totalSnow=GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);

            _logger.LogInformation($"Zip: {zip} over last {days} days:" +
                $"total snow: {totalSnow}, rain: {totalRain}");

            var tempData = await FetchTemperatureData(httpclient, zip, days);
            var averageHigh = tempData.Average(t => t.TempHigh);
            var averageLow = tempData.Average(t => t.TempLow);
            _logger.LogInformation($"Zip: {zip} over last {days} days:" +
              $"Low Temp: {averageLow}, High Temp: {averageHigh}");

            var weatherReport = new WeatherReport
            {
                AverageHigh = averageHigh,
                AverageLow= averageLow,
                RainFallTotalInches=totalRain,
                SnowTotalInches= totalSnow,
                ZipCode=zip,
                CreatedOn=DateTime.UtcNow
            };

            // TODO: Use cached weather report instead of making round trips when possible
            _weatherReportDbContext.Add(weatherReport);
            await _weatherReportDbContext.SaveChangesAsync();

            return weatherReport;
        }

        private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData)
        {
            var totalSnow = precipData
                .Where(p => p.WeatherType == "snow")
                .Sum(p => p.AmountInches);
            return Math.Round(totalSnow,1);
        }
        private static decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData)
        {
            var totalRain = precipData
                .Where(p => p.WeatherType == "rain")
                .Sum(p => p.AmountInches);
            return Math.Round(totalRain,1);
        }
        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient client,string zip,int days)
        {
            var endPoint = BuildTempServiceEndPoint(zip, days);

            var temperatureRecords = await client.GetAsync(endPoint);
            var temperatureData= await temperatureRecords
                .Content
                .ReadFromJsonAsync<List<TemperatureModel>>();
            return temperatureData?? new List<TemperatureModel>();
        }

        private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient client, string zip, int days)
        {

            var endPoint = BuildPrecipitationServiceEndPoint(zip, days);

            var precipRecords = await client.GetAsync(endPoint);
            var jsonSerializationOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            var preciptData = await precipRecords
                .Content
                .ReadFromJsonAsync<List<PrecipitationModel>>(jsonSerializationOptions);
            return preciptData ?? new List<PrecipitationModel>();
        }

        private string BuildTempServiceEndPoint(string zip, int days)
        {
            var tempServiceProtocol = _weatherDataConfig.TempDataProtocol;
            var tempServiceHost = _weatherDataConfig.TempDataHost;
            var tempServicePort = _weatherDataConfig.TempDataPort;
            return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/{zip}?days={days}";
        }

        private string BuildPrecipitationServiceEndPoint(string zip, int days)
        {
            var precipServiceProtocol = _weatherDataConfig.PrecipDataProtocol;
            var precipServiceHost = _weatherDataConfig.PrecipDataHost;
            var precipServicePort = _weatherDataConfig.PrecipDataPort;
            return $"{precipServiceProtocol}://{precipServiceHost}:{precipServicePort}/observation/{zip}?days={days}";
        }
        

    }

 
}
