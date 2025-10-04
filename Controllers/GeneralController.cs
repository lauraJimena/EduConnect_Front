using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class GeneralController : Controller
    {
        
        private readonly GeneralService _generalService = new GeneralService();


        public IActionResult IniciarSesion()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Registro()
        {
            return View(new CrearUsuarioDto());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Registro(CrearUsuarioDto dto, CancellationToken ct)
        {
            var (ok, msg) = await _generalService.RegistrarUsuario(dto, ct);

            if (ok)
            {
                TempData["RegisterSuccess"] = msg;
                return RedirectToAction("IniciarSesion", "General");
            }

            // Mostrar el texto que vino del back (p.ej. "Error interno: …")
            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(IniciarSesionDto dto, CancellationToken ct)
        {
            var (ok, msg, usuario) = await _generalService.IniciarSesion(dto, ct);

            if (ok && usuario != null)
            {
                // Guarda todo el usuario
                HttpContext.Session.SetObject("Usuario", usuario);  

                // Aquí ya esta el usuario autenticado desde el back
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                HttpContext.Session.SetInt32("IdRol", usuario.IdRol);

                switch (usuario.IdRol)
                {
                    case 1:
                        return RedirectToAction("PanelTutorado", "Tutorado");
                    case 2:
                        return RedirectToAction("PanelTutor", "Tutor");
                    case 3:
                        return RedirectToAction("PanelAdministrador", "Administrador");
                    case 4:
                        return RedirectToAction("PanelCoordinador", "Coordinador");
                    default:
                        return RedirectToAction("Index", "Home");
                }

            }

            // Si hubo error, se envia a la vista con popup
            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }
    }
}
