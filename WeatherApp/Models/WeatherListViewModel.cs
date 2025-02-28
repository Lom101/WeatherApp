using WeatherApp.Entity;

namespace WeatherApp.Models;

public class WeatherListViewModel
{
    public List<WeatherRecord> WeatherRecords { get; set; } = new List<WeatherRecord>();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int SelectedYear { get; set; }
    public int SelectedMonth { get; set; }
}
