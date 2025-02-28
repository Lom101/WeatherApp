namespace WeatherApp.Service.Interface;

public interface IExcelParserService
{
    public Task<bool> ProcessExcelFileAsync(Stream fileStream);
}