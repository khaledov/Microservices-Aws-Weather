using CloudWeather.Report.DataAccess;

namespace CloudWeather.Report.BusinessLogic
{

    /// <summary>
    /// Weather report service
    /// </summary>
    public interface IWeatherReportAggregator
    {
        /// <summary>
        /// Builds the report.
        /// </summary>
        /// <param name="zip">The zip.</param>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        public Task<WeatherReport> BuildReport(string zip, int days);


    }
}