using Microsoft.EntityFrameworkCore;
using WeatherApp.Entity;

namespace WeatherApp.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) 
        : base(options) { }
    
    public DbSet<WeatherRecord> WeatherRecords { get; set; }
}