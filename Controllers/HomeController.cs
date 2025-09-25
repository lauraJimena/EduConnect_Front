using System.Diagnostics;
using EduConnect_Front.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Inicio()
        {
            return View();
        }
        public IActionResult Registro()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
