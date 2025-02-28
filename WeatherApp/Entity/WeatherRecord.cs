using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Entity;

/// <summary>
/// Класс, представляющий запись о погодных данных.
/// </summary>
public class WeatherRecord
{
    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Дата записи погодных данных.
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Время записи погодных данных.
    /// </summary>
    public TimeOnly? Time { get; set; }
    
    /// <summary>
    /// Температура воздуха в градусах Цельсия.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Относительная влажность воздуха в процентах.
    /// </summary>
    public double? Humidity { get; set; }
    
    /// <summary>
    /// Точка росы - Td
    /// </summary>
    public double? DewPoint { get; set; }
    
    /// <summary>
    /// Атмосферное давление в миллиметрах ртутного столба.
    /// </summary>
    public double? Pressure { get; set; }

    /// <summary>
    /// Направление ветра
    /// </summary>
    public string? WindDirection { get; set; } = string.Empty;

    /// <summary>
    /// Скорость ветра в метрах в секунду.
    /// </summary>
    public double? WindSpeed { get; set; }
    
    /// <summary>
    /// Облачность в процентах
    /// </summary>
    public int? Cloudiness { get; set; }
    
    /// <summary>
    /// Нижняя граница облачности в метрах
    /// </summary>
    public int? LowerCloudLimit { get; set; }
    
    /// <summary>
    /// Горизонтальная видимость в километрах
    /// </summary>
    public int? HorizontalVisibility { get; set; }

    /// <summary>
    /// Явления погоды (например, "Дождь", "Снег", "Ясно").
    /// </summary>
    public string? WeatherPhenomena { get; set; } = string.Empty;
}