using Microsoft.EntityFrameworkCore;

namespace WebApplication2
{
    public class asd : DbContext
    {
        public DbSet<WeatherForecast> Users { get; set; }
    }
}
