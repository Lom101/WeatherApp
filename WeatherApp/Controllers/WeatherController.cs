using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Entity;
using WeatherApp.Models;
using WeatherApp.Service.Interface;
using Microsoft.Extensions.Logging;

namespace WeatherApp.Controllers
{
    [Route("[controller]")]
    public class WeatherController : Controller
    {
        private readonly IExcelParserService _excelParserService;
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherController> _logger;

        // Константы для настройки
        private const int PageSize = 100; // Количество записей на странице
        private const long MaxFileSize = 10 * 1024 * 1024; // Максимальный размер файла (10 MB)
        private const string AllowedContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // Формат .xlsx

        public WeatherController(IExcelParserService excelParserService, WeatherDbContext context, ILogger<WeatherController> logger)
        {
            _excelParserService = excelParserService ?? throw new ArgumentNullException(nameof(excelParserService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Возвращает страницу со списком записей о погоде с пагинацией и фильтрацией по году/месяцу.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int? year = null, int? month = null)
        {
            if (page <= 0)
            {
                return BadRequest("Номер страницы должен быть больше 0");
            }

            // Формируем запрос к базе данных
            IQueryable<WeatherRecord> query = _context.WeatherRecords.AsQueryable();

            if (year.HasValue && year > 0)
            {
                query = query.Where(w => w.Date.Year == year.Value);
            }

            if (month.HasValue && month > 0 && month <= 12)
            {
                query = query.Where(w => w.Date.Month == month.Value);
            }

            // Подсчитываем общее количество записей
            int totalRecords = await query.CountAsync();

            // Получаем данные для текущей страницы
            var weatherData = await query
                .OrderBy(w => w.Date)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Создаем модель представления
            var viewModel = new WeatherListViewModel
            {
                WeatherRecords = weatherData,
                Page = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize),
                SelectedYear = year ?? 0,
                SelectedMonth = month ?? 0
            };

            return View(viewModel);
        }

        /// <summary>
        /// Отображает форму для загрузки файла Excel.
        /// </summary>
        [HttpGet("Upload")]
        public IActionResult ShowUploadForm()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает загруженный файл Excel и сохраняет данные в базу данных.
        /// </summary>
        [HttpPost("Upload")]
        public async Task<IActionResult> HandleFileUpload(IEnumerable<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                ModelState.AddModelError(nameof(files), "Выберите хотя бы один файл для загрузки.");
                return View("ShowUploadForm");
            }

            foreach (var file in files)
            {
                try
                {
                    // Проверяем файл
                    if (!ValidateFile(file))
                    {
                        continue; // Пропускаем невалидные файлы
                    }

                    _logger.LogInformation($"Обработка файла {file.FileName}...");

                    using (var stream = file.OpenReadStream())
                    {
                        bool success = await _excelParserService.ProcessExcelFileAsync(stream);

                        if (!success)
                        {
                            _logger.LogError($"Ошибка при обработке файла \"{file.FileName}\".");
                            ModelState.AddModelError(nameof(files), $"Ошибка при обработке файла \"{file.FileName}\".");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(files), $"Произошла ошибка при обработке файла \"{file.FileName}\": {ex.Message}");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("ShowUploadForm");
            }

            _logger.LogInformation("Файлы успешно загружены.");
            return RedirectToAction("Index"); // Перенаправляем на главную страницу после успешной загрузки
        }

        /// <summary>
        /// Проверяет корректность файла.
        /// </summary>
        private bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(nameof(file), $"Файл \"{file.FileName}\" пустой.");
                return false;
            }

            if (file.ContentType != AllowedContentType)
            {
                ModelState.AddModelError(nameof(file), $"Файл \"{file.FileName}\" имеет неверный формат. Загрузите файл в формате .xlsx.");
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                ModelState.AddModelError(nameof(file), $"Размер файла \"{file.FileName}\" превышает допустимый лимит (10 MB).");
                return false;
            }

            return true;
        }
    }
}