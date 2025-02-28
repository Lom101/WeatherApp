using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}