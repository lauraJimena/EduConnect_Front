using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EduConnect_Front.Controllers
{
    public class GeneralController : Controller
    {
        
        private readonly GeneralService _generalService = new GeneralService();
        private readonly AdministradorService _administradorService = new AdministradorService();
        private readonly TutorService _tutorService = new TutorService();


        public IActionResult IniciarSesion()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> RegistroAsync()
        {
            AdministradorService _administradorService = new AdministradorService();
            var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
            var carreras = await _administradorService.ObtenerCarrerasAsync();
            ViewBag.TipoIdent = tipoIdent;
            ViewBag.Carreras = carreras;
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
            // Cargar carreras para el dropdown
            AdministradorService _administradorService = new AdministradorService();
            var carreras = await _administradorService.ObtenerCarrerasAsync();

            var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
            ViewBag.TipoIdent = tipoIdent;
            ViewBag.Carreras = carreras;
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(IniciarSesionDto dto, CancellationToken ct)
        {
            var (ok, msg, usuario) = await _generalService.IniciarSesion(dto, ct);

            if (ok && usuario != null)
            {
               
                //HttpContext.Session.SetString("Token", usuario.Token ?? string.Empty);
              
                var token = usuario.Token ?? string.Empty;
                HttpContext.Session.SetString("Token", token);

                //Decodifica el token para obtener el ID
                var idUsuario = JwtUtility.ObtenerIdUsuarioDesdeToken(usuario.Token ?? "");

                if (idUsuario == null)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo obtener el ID del usuario desde el token.");
                    return View(dto);
                }

                //Llama a la API para obtener la información completa del usuario
                var (okUsuario, msgUsuario, infoUsuario) = await _administradorService.ObtenerUsuarioPorIdAsync(idUsuario.Value, usuario.Token ?? "", ct);

                if (!okUsuario || infoUsuario == null)
                {
                    ModelState.AddModelError(string.Empty, msgUsuario);
                    return View(dto);
                }

                //Guarda el usuario completo en sesión
                HttpContext.Session.SetObject("Usuario", infoUsuario);
                HttpContext.Session.SetInt32("IdRol", infoUsuario.IdRol);
                HttpContext.Session.SetInt32("IdUsu", infoUsuario.IdUsu);
                HttpContext.Session.SetString("UsuarioNombre", infoUsuario.Nombre ?? "");
                HttpContext.Session.SetString("AvatarUrl", infoUsuario.Avatar ?? "/img/avatars/avatar3.png");
                // Validar si es tutor
                if (okUsuario && infoUsuario.IdRol == 2)
                {
                    var (tieneMaterias, msgValidacion) = await _tutorService.ValidarMateriasTutorAsync(infoUsuario.IdUsu, token);

                    if (!tieneMaterias)
                    {
                        TempData["Advertencia"] = "Aún no tienes registradas las materias que dominas. Por favor complétalas para que los estudiantes puedan ver tu perfil.";
                        return RedirectToAction("RegistrarMaterias", "Tutor");
                    }

                    return RedirectToAction("PanelTutor", "Tutor");
                }

                //Redirige según el rol
                switch (infoUsuario.IdRol)
                {
                    case 1: return RedirectToAction("PanelTutorado", "Tutorado");
                   
                    case 3: return RedirectToAction("PanelAdministrador", "Administrador");
                    case 4: return RedirectToAction("PanelCoordinador", "Coordinador");
                    default: return RedirectToAction("Inicio", "Home");
                }
            }

            // Si hubo error, mostrar mensaje
            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }
    }
}
