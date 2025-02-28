using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Service;
using WeatherApp.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddTransient<IExcelParserService, ExcelParserService>();

var app = builder.Build();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
