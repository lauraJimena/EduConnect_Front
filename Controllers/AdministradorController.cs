using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly AdministradorService _administradorService = new AdministradorService();
        // GET: TutoradoController
        [HttpGet]
        public IActionResult RegistrarUsuarios()
        {
            // Estado 1 por defecto (activo)
            var model = new CrearUsuarioDto { IdEstado = 1 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarUsuarios(CrearUsuarioDto dto, CancellationToken ct)
        {
            var (ok, msg) = await _administradorService.RegistrarUsuario(dto, ct);

            if (ok)
            {
                TempData["AdminRegisterOk"] = msg; // para popup de éxito
                
                return RedirectToAction("PanelAdministrador", "Administrador");
            }

            // Deja los datos en pantalla y muestra el error en popup
            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }

        public async Task<IActionResult> PanelAdministrador()
        {
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                var idUsuario = HttpContext.Session.GetInt32("IdUsu");

                if (idUsuario == null)
                {
                    TempData["Error"] = "No se encontró información del usuario actual.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                //Volver a consultar al backend por los datos actualizados
                var usuario = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsuario.Value, token);

                if (usuario == null)
                {
                    TempData["Error"] = "No se pudo cargar el perfil del tutorado.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                return View(usuario);




            }
        }


        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id, CancellationToken ct)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                var (ok, msg, usuario) = await _administradorService.ObtenerUsuarioPorIdPerfil(id, token, ct);

                if (!ok || usuario == null)
                {
                    TempData["Error"] = msg ?? "Usuario no encontrado.";
                    return RedirectToAction("ConsultarUsuarios");
                }

                var tipoIdent = await new GeneralService().ObtenerTipoIdentAsync();
                var carreras = await _administradorService.ObtenerCarrerasAsync();

                ViewBag.TipoIdent = tipoIdent;
                ViewBag.Carreras = carreras;

                // ✅ No tocar TempData aquí
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ConsultarUsuarios");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(ActualizarUsuarioDto dto)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "No se encontró el token de autenticación. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // ✅ Desempaquetamos la tupla correctamente
                var (ok, msg) = await _administradorService.ActualizarUsuarioAsync(dto, token);

                if (ok)
                {
                    // Solo guardamos el mensaje de texto
                    TempData["Success"] = msg;
                }
                else
                {
                    TempData["Error"] = msg;
                }

                // ✅ Redirigimos al GET (para mostrar el popup si aplica)
                return RedirectToAction("EditarUsuario", new { id = dto.IdUsu });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("EditarUsuario", new { id = dto.IdUsu });
            }
        }


        //[HttpGet]
        //public async Task<IActionResult> ConsultarUsuarios(CancellationToken ct)
        //{
        //    var (ok, msg, items) = await _administradorService.ConsultarUsuarios(ct);

        //    if (!ok)
        //    {
        //        ModelState.AddModelError(string.Empty, msg);
        //        items = new List<ListadoUsuariosDto>();
        //    }

        //    return View(items);
        //}
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int idUsuario, CancellationToken ct)
        {
            var (ok, msg) = await _administradorService.EliminarUsuarioAsync(idUsuario, ct);

            if (ok)
                return Ok(msg);
            else
                return BadRequest(msg);
        }
        [HttpGet]

        public async Task<IActionResult> ConsultarUsuarios(int? idRol, int? idEstado, string? numIdent)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró token de sesión. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (ok, msg, usuarios) = await _administradorService.ObtenerUsuariosAsync(token, idRol, idEstado, numIdent);

            if (!ok)
            {
                TempData["Error"] = msg;
                return View(new List<ListadoUsuariosDto>());
            }

           
            return View(usuarios);
        }



    }
}
