using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class TutoradoController : Controller
    {
        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly AdministradorService _administradorService = new AdministradorService();
        private readonly GeneralService _generalService = new GeneralService();

        // GET: TutoradoController
        [HttpGet]
        public async Task<IActionResult> PanelTutorado()
        {
            
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // ✅ Obtener el ID del usuario actual desde la sesión o el token
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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("IniciarSesion", "General");
            }

        }
        public ActionResult RankingTutores()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> HistorialTutorias([FromQuery] List<int>? idsEstado)
        {
            // 🔹 Recuperar el usuario y el token de la sesión
            var IdUsu = HttpContext.Session.GetInt32("IdUsu");
            var token = HttpContext.Session.GetString("Token");

            if (IdUsu == null || string.IsNullOrEmpty(token))
                return RedirectToAction("IniciarSesion", "General");

            //ViewBag.Usuario = usuario; // Para saludo en la vista

            // 🔹 Llamar al servicio con el token y el Id del usuario (tutorado)
            var (ok, msg, datos) = await _tutoradoService.ObtenerHistorialAsync(
                IdUsu.Value,
                token,          // ✅ Agregar el token aquí
                idsEstado
            );

            // 🔹 Validar respuesta
            if (!ok)
            {
                ViewData["Error"] = msg;
                return View(new List<HistorialTutoriaDto>());
            }

            return View(datos ?? new List<HistorialTutoriaDto>());
        }

        // Muestra formulario vacío y tabla sin resultados
        [HttpGet]
        public async Task<IActionResult> BusquedaTutores(CancellationToken ct)
        {
            var filtrosVacios = new BuscarTutorDto
            {
                // todo null/"" -> el back debe devolver todos
                Nombre = null,
                MateriaNombre = null,
                Semestre = null,
                CarreraNombre = null,
                IdEstado = null
            };

            var (ok, msg, items) = await _tutoradoService.BuscarTutoresAsync(filtrosVacios, ct);
            if (!ok) ModelState.AddModelError(string.Empty, msg);

            return View(items); // tu vista ya recibe List<ObtenerTutorDto>
        }

        // Envía filtros al back (POST /Tutor/obtener) y muestra resultados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusquedaTutores(BuscarTutorDto filtros, CancellationToken ct)
        {
            var (ok, msg, items) = await _tutoradoService.BuscarTutoresAsync(filtros, ct);
            if (!ok) ModelState.AddModelError(string.Empty, msg);
            return View(items);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarTutorado(EditarPerfilDto perfil)
        {
            try { 
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró el token de autenticación. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var mensaje = await _tutoradoService.ActualizarPerfilAsync(perfil, token);
            TempData["Success"] = mensaje;
                TempData["RedirectToPanel"] = true;

                var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
                var carreras = await _administradorService.ObtenerCarrerasAsync();
                ViewBag.TipoIdent = tipoIdent;
                ViewBag.Carreras = carreras;
                return View(perfil);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Carreras = await _generalService.ObtenerCarrerasAsync();
                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                return View(perfil);
            }
        }
       
        [HttpGet]
        public async Task<IActionResult> EditarTutorado()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var idUsu = HttpContext.Session.GetInt32("IdUsu");
                

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // 🔹 Llama al service que ya devuelve EditarPerfilDto
                var modelo = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsu.Value, token);

                if (modelo == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("ConsultarUsuarios");
                }
                var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
                var carreras = await _administradorService.ObtenerCarrerasAsync();
                ViewBag.TipoIdent = tipoIdent;
                ViewBag.Carreras = carreras;
                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ConsultarUsuarios");
            }
        }
        [HttpGet]
        public async Task<ActionResult> SolicitudesTutoriasAsync()
        {

            var token = HttpContext.Session.GetString("Token");
            var idTutorado = HttpContext.Session.GetInt32("IdUsu");

            if (string.IsNullOrEmpty(token) || idTutorado == null)
                return RedirectToAction("IniciarSesion", "General");

            var filtro = new FiltroSolicitudesDto
            {
                IdTutorado = idTutorado.Value,
                Estados = new List<int> { 3, 4, 5 }
            };

            var solicitudes = await _tutoradoService.ObtenerSolicitudesTutoriasAsync(filtro, token);
            return View(solicitudes);

        }
        [HttpPost]
        public async Task<IActionResult> SolicitudesTutorias(FiltroSolicitudesDto filtro, CancellationToken ct)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var idTutorado = HttpContext.Session.GetInt32("IdUsu");

                if (string.IsNullOrEmpty(token) || idTutorado == null)
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // 🔹 Asignar el IdTutorado que pide el backend
                filtro.IdTutorado = idTutorado.Value;

                // Si no marcó ningún estado, usar 3, 4, 5 por defecto
                if (filtro.Estados == null || !filtro.Estados.Any())
                    filtro.Estados = new List<int> { 3, 4, 5 };

                var solicitudes = await _tutoradoService.ObtenerSolicitudesTutoriasAsync(filtro, token, ct);

                return View("SolicitudesTutorias", solicitudes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("PanelTutorado");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormSolicitudTutoria(SolicitudTutoriaRespuestaDto modelo)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (success, message) = await _tutoradoService.CrearSolicitudTutoriaAsync(modelo, token);

            if (success)
            {
                TempData["Success"] = message;
                return View("FormSolicitudTutoria", modelo);
            }

            TempData["Error"] = message;
            return View("FormSolicitudTutoria", modelo);
        }
        [HttpGet]
        public IActionResult FormSolicitudTutoria(int idTutor, int idMateria)
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutorado = HttpContext.Session.GetInt32("IdUsu");

            // Crear un nuevo modelo para la vista del formulario
            var modelo = new SolicitudTutoriaRespuestaDto
            {
                IdTutor = idTutor,
                IdTutorado= idTutorado.Value,
                IdMateria = idMateria
                //Fecha = DateTime.Today 
            };

            return View("FormSolicitudTutoria", modelo);
        }










    }
}
