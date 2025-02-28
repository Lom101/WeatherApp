using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WeatherApp.Data;
using WeatherApp.Entity;
using WeatherApp.Service.Interface;

namespace WeatherApp.Service
{
    public class ExcelParserService : IExcelParserService
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<ExcelParserService> _logger;

        public ExcelParserService(WeatherDbContext context, ILogger<ExcelParserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Обрабатывает файл Excel и сохраняет данные в базу данных.
        /// </summary>
        public async Task<bool> ProcessExcelFileAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Начинаем обработку файла Excel.");
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                var weatherRecords = new List<WeatherRecord>();

                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIndex);

                    for (int rowIndex = 5; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);
                        if (row == null) continue;

                        try
                        {
                            var record = ParseRow(row);
                            if (record != null)
                            {
                                weatherRecords.Add(record);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Ошибка обработки строки {rowIndex}: {ex.Message}");
                        }
                    }
                }

                _context.WeatherRecords.AddRange(weatherRecords);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Файл успешно обработан.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при обработке файла Excel.");
                return false;
            }
        }

        private WeatherRecord? ParseRow(IRow row)
        {
            try
            {
                // Извлекаем значения ячеек
                var dateCell = row.GetCell(0)?.ToString();
                var timeCell = row.GetCell(1)?.ToString();
                var temperature = ToDoubleOrNull(row.GetCell(2));
                var humidity = ToDoubleOrNull(row.GetCell(3));
                var dewPoint = ToDoubleOrNull(row.GetCell(4));
                var pressure = ToDoubleOrNull(row.GetCell(5));
                var windDirection = row.GetCell(6)?.ToString() ?? string.Empty;
                var windSpeed = ToDoubleOrNull(row.GetCell(7));
                var cloudiness = ToIntOrNull(row.GetCell(8));
                var lowerCloudLimit = ToIntOrNull(row.GetCell(9));
                var horizontalVisibility = ToIntOrNull(row.GetCell(10));
                var weatherPhenomena = row.GetCell(11)?.ToString() ?? string.Empty;

                // Парсим дату и время
                if (string.IsNullOrWhiteSpace(dateCell))
                {
                    _logger.LogWarning($"Пропущена дата в строке {row.RowNum + 1}.");
                    return null;
                }

                DateTime date;
                if (!DateTime.TryParse(dateCell, out date))
                {
                    _logger.LogWarning($"Неверный формат даты в строке {row.RowNum + 1}: {dateCell}");
                    return null;
                }

                TimeOnly time = default;
                if (!string.IsNullOrWhiteSpace(timeCell) && !TimeOnly.TryParse(timeCell, out time))
                {
                    _logger.LogWarning($"Неверный формат времени в строке {row.RowNum + 1}: {timeCell}");
                }

                // Создаем объект WeatherRecord
                return new WeatherRecord
                {
                    Date = new DateOnly(date.Year, date.Month, date.Day),
                    Time = time,
                    Temperature = temperature ?? null,
                    Humidity = humidity ?? null,
                    DewPoint = dewPoint ?? null,
                    Pressure = pressure ?? null,
                    WindDirection = windDirection,
                    WindSpeed = windSpeed ?? null,
                    Cloudiness = cloudiness ?? null,
                    LowerCloudLimit = lowerCloudLimit ?? null,
                    HorizontalVisibility = horizontalVisibility ?? null,
                    WeatherPhenomena = weatherPhenomena
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка обработки строки {row?.RowNum + 1}");
                return null;
            }
        }

        private double? ToDoubleOrNull(ICell cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
                return null;

            if (double.TryParse(cell.ToString(), out double result))
                return result;

            return null;
        }

        private int? ToIntOrNull(ICell cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
                return null;

            if (int.TryParse(cell.ToString(), out int result))
                return result;

            return null;
        }
    }
}