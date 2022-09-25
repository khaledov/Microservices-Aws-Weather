using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Report.DataAccess
{
    public class WeatherReportDbContext:DbContext
    {
        public WeatherReportDbContext()
        {

        }
        public WeatherReportDbContext(DbContextOptions options):
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>().ToTable("weather_report");
        }
    }
}
