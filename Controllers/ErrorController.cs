using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        // ModelState.IsValid no aplica: este método no recibe datos del usuario, solo un código de error HTTP.
#pragma warning disable S6967 // ModelState.IsValid should be checked in controller actions
        public IActionResult HandleError(int statusCode)
#pragma warning restore S6967
        {
            if (!ModelState.IsValid)
            {
                return View("ErrorGenerico");
            }

            switch (statusCode)
            {
                case 404:
                    return View("Error404");
                case 500:
                    return View("Error500");
                default:
                    return View("ErrorGenerico");
            }
        }
    }
}
