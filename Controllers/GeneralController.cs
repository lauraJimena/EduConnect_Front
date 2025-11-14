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
        public IActionResult AccesoDenegado()
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
        public async Task<IActionResult> Registro(CrearUsuarioDto dto, string token, CancellationToken ct)
        {

            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(dto);
            }
            var (ok, msg) = await _generalService.RegistrarUsuario(dto, token, ct);

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
            if (!ModelState.IsValid)
            {
                // Devuelve la vista con los errores de validación del modelo
                return View(dto);
            }
            var (ok, msg, usuario) = await _generalService.IniciarSesion(dto, ct);

                if (ok && usuario != null)
            {
               
                //HttpContext.Session.SetString("Token", usuario.Token ?? string.Empty);
              
                var token = usuario.Token ?? string.Empty;
                HttpContext.Session.SetString("Token", token);

                //Decodifica el token para obtener el ID
                var idUsuario = JwtUtility.ObtenerIdUsuarioDesdeToken(usuario.Token ?? "");
                HttpContext.Session.SetInt32("IdUsu", (int)idUsuario);

                // verificar si debe cambiar la contraseña
                    if (usuario.DebeActualizarPassword)
                {
                    TempData["MostrarPopup"] = true;
                    // Redirige a la vista de cambio de contraseña
                    return RedirectToAction("CambiarContras", "General");
                }
        
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
                //HttpContext.Session.SetObject("Usuario", infoUsuario);
                HttpContext.Session.SetInt32("IdRol", infoUsuario.IdRol);
                HttpContext.Session.SetInt32("IdUsu", infoUsuario.IdUsu);
                HttpContext.Session.SetString("UsuarioNombre", infoUsuario.Nombre ?? "");
                HttpContext.Session.SetString("AvatarUrl", infoUsuario.Avatar ?? "avatar1.png");
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
        [HttpGet]
        public IActionResult CambiarContras()
        {
            ViewBag.MostrarPopup = TempData["MostrarPopup"];
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CambiarContrasena(string nuevaPassword, CancellationToken ct)
        {
            // 🔹 Recuperar datos desde la sesión
            var idUsuario = HttpContext.Session.GetInt32("IdUsu");
            var token = HttpContext.Session.GetString("Token");

            if (idUsuario == null || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró la información del usuario en sesión. Vuelve a iniciar sesión.";
                return RedirectToAction("IniciarSesion", "General");
            }

            // 🔹 Llamar al servicio
            var (ok, msg) = await _generalService.ActualizarPasswordAsync(idUsuario.Value, token, nuevaPassword, ct);

            if (ok)
            {
                TempData["ContrasActualizada"] = msg;
                return RedirectToAction("IniciarSesion", "General");
            }

            TempData["ErrorContras"] = msg;
            return RedirectToAction("CambiarContras");
        }
        [HttpGet]
        public IActionResult OlvidarContrasena()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EnviarCorreoRecuperacion(string correo, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                TempData["Error"] = "Debe ingresar un correo válido.";
                return RedirectToAction("OlvidarContrasena");
            }

            var (ok, msg) = await _generalService.EnviarCorreoRecuperacionAsync(correo, ct);

            if (ok)
                TempData["Exito"] = "Se ha enviado un enlace de recuperación a tu correo.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("OlvidarContrasena");
        }
        [HttpGet]
        public IActionResult RestablecerContrasena(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("OlvidarContrasena", "General");

            ViewBag.Token = token; // se pasa a la vista
            return View();          // muestra el formulario de nueva contraseña
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContrasena(RestablecerContrasenaDto dto, CancellationToken ct)
        {

            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(dto);
            }
            var (ok, msg) = await _generalService.RestablecerContrasenaAsync(dto, ct);

            if (ok)
            {
                TempData["Exito"] = msg;
                return RedirectToAction("IniciarSesion");
            }

            TempData["Error"] = msg;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            TempData["CerrarSesion"] = "Sesión cerrada correctamente.";
            return RedirectToAction("IniciarSesion", "General");
        }


    }
}
