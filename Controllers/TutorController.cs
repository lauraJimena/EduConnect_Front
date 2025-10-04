using EduConnect_Front.Dtos;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class TutorController : Controller
    {
        [HttpGet]
        public IActionResult PanelTutor()
        {
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            if (usuario == null)
            {
                // No hay sesión -> volver a iniciar sesión
                return RedirectToAction("IniciarSesion", "General");
            }

            return View(usuario); // Pasa el DTO completo a la vista
        }
    }
}
