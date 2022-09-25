using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temprature.DataAccess
{
    public class TempDbContext:DbContext
    {
        public TempDbContext() { }
        public TempDbContext(DbContextOptions option):
            base(option) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(b => b.ToTable("temperature"));
        }

    }
}
