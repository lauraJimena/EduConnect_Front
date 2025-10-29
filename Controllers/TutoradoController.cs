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
        [ValidarRol(1)]
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


        [HttpGet]
        [ValidarRol(1)]
        public async Task<IActionResult> HistorialTutorias([FromQuery] List<int>? idsEstado)
        {
           
            var IdUsu = HttpContext.Session.GetInt32("IdUsu");
            var token = HttpContext.Session.GetString("Token");

            if (IdUsu == null || string.IsNullOrEmpty(token))
                return RedirectToAction("IniciarSesion", "General");

            //ViewBag.Usuario = usuario; // Para saludo en la vista
           
            var (ok, msg, datos) = await _tutoradoService.ObtenerHistorialAsync(
                IdUsu.Value,
                token,         
                idsEstado
            );

            if (!ok)
            {
                ViewData["Error"] = msg;
                return View(new List<HistorialTutoriaDto>());
            }

            return View(datos ?? new List<HistorialTutoriaDto>());
        }

        // GET: /Tutorado/BusquedaTutores
      
        [HttpGet]
        [ValidarRol(1)]
        public async Task<IActionResult> BusquedaTutores(
            int page = 1,
            string Nombre = "",
            string CarreraNombre = "",
            string MateriaNombre = "",
            string Semestre = "",
            int? IdEstado = null)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var filtros = new BuscarTutorDto
            {
                Page = page < 1 ? 1 : page,
                PageSize = 4, // mostramos 4
                Nombre = (Nombre ?? "").Trim(),
                CarreraNombre = (CarreraNombre ?? "").Trim(),
                MateriaNombre = (MateriaNombre ?? "").Trim(),
                Semestre = (Semestre ?? "").Trim(),
                IdEstado = IdEstado
               
            };

            // Ideal: el backend devuelve PageSize+1 (5) para detectar "siguiente"
            var (ok, msg, tutores) = await _tutoradoService.BuscarTutoresAsync(filtros, token);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, msg ?? "No se pudieron obtener tutores.");
                tutores = new List<ObtenerTutorDto>();
            }

            bool hasMore = tutores.Count > filtros.PageSize;
            var tutoresMostrados = tutores.Take(filtros.PageSize).ToList();

            ViewBag.Page = filtros.Page;
            ViewBag.HasMore = hasMore;
            ViewBag.Filtros = filtros;

            return View(tutoresMostrados);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(1)]
        public IActionResult BusquedaTutores(BuscarTutorDto filtros)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            // Normaliza y fuerza la página 1 al aplicar filtros
            return RedirectToAction(nameof(BusquedaTutores), new
            {
                page = 1,
                Nombre = filtros?.Nombre?.Trim() ?? "",
                CarreraNombre = filtros?.CarreraNombre?.Trim() ?? "",
                MateriaNombre = filtros?.MateriaNombre?.Trim() ?? "",
                Semestre = filtros?.Semestre?.Trim() ?? "",
                IdEstado = filtros?.IdEstado
            });
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditarTutorado(EditarPerfilDto perfil)
        //{
        //    try { 
        //    var token = HttpContext.Session.GetString("Token");
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        TempData["Error"] = "No se encontró el token de autenticación. Inicia sesión nuevamente.";
        //        return RedirectToAction("IniciarSesion", "General");
        //    }

        //    var mensaje = await _tutoradoService.ActualizarPerfilAsync(perfil, token);
        //    TempData["Success"] = mensaje;
        //        TempData["RedirectToPanel"] = true;

        //        var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
        //        var carreras = await _administradorService.ObtenerCarrerasAsync();
        //        ViewBag.TipoIdent = tipoIdent;
        //        ViewBag.Carreras = carreras;
        //        return View(perfil);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Message;
        //        ViewBag.Carreras = await _generalService.ObtenerCarrerasAsync();
        //        ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
        //        return View(perfil);
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditarTutorado(CancellationToken ct)
        //{
        //    var token = HttpContext.Session.GetString("Token");
        //    var idUsu = HttpContext.Session.GetInt32("IdUsu");

        //    if (string.IsNullOrEmpty(token) || !idUsu.HasValue)
        //        return RedirectToAction("IniciarSesion", "General");

        //    var modelo = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsu.Value, token, ct);
        //    if (modelo == null)
        //    {
        //        TempData["Error"] = "Usuario no encontrado.";
        //        return RedirectToAction("PanelTutorado");
        //    }

        //    ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
        //    ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();
        //    return View(modelo);
        //}
     
        [HttpGet]
        [ValidarRol(1)]
        public async Task<IActionResult> EditarTutorado()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var idUsu = HttpContext.Session.GetInt32("IdUsu");

                if (string.IsNullOrEmpty(token) || idUsu == null)
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // 🔸 Obtener datos del tutorado (incluye avatar)
                var modelo = await _tutoradoService.ObtenerTutoradoParaEditarAsync(idUsu.Value, token);

                if (modelo == null)
                {
                    TempData["Error"] = "No se encontró la información del tutorado.";
                    return RedirectToAction("PanelTutorado");
                }


                // 🔸 Combos de apoyo
                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("PanelTutorado");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(1)]
        public async Task<IActionResult> EditarTutorado(EditarPerfilDto perfil)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                var mensaje = await _tutoradoService.ActualizarPerfilAsync(perfil, token);

                TempData["Success"] = mensaje;
                TempData["RedirectToPanel"] = true;

                // Vuelve a obtener los datos actualizados del backend
                var modeloActualizado = await _tutoradoService.ObtenerTutoradoParaEditarAsync(perfil.IdUsu, token);

                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();
                HttpContext.Session.SetString("AvatarUrl", perfil.Avatar);
                HttpContext.Session.SetString("UsuarioNombre", perfil.Nombre);

                return View(modeloActualizado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();

                return View(perfil);
            }
        }

        [HttpGet]
        [ValidarRol(1)]
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
        [ValidarRol(1)]
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
        [ValidarRol(1)]
        public async Task<IActionResult> FormSolicitudTutoria(SolicitudTutoriaRespuestaDto modelo)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (success, message, idTutoria) = await _tutoradoService.CrearSolicitudTutoriaAsync(modelo, token);

            if (success)
            {
                // 📩 Enviar correo de confirmación automáticamente
                try
                {
                    bool correoEnviado = await _tutoradoService.EnviarCorreoConfirmacionTutoriaAsync(token, idTutoria);

                    if (correoEnviado)
                        TempData["Success"] = $"{message} ✅ Se envió un correo de confirmación al tutorado.";
                    else
                        TempData["Warning"] = $"{message} ⚠️ La solicitud se creó, pero no se pudo enviar el correo.";
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = $"{message} ⚠️ La solicitud se creó, pero hubo un error al enviar el correo: {ex.Message}";
                }

                return View("FormSolicitudTutoria", modelo);
            }

            TempData["Error"] = message;
            return View("FormSolicitudTutoria", modelo);
        }
        [HttpGet]
        [ValidarRol(1)]
        public IActionResult FormSolicitudTutoria(int idTutor, int idMateria, string nombreMateria)
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutorado = HttpContext.Session.GetInt32("IdUsu");

            // Crear un nuevo modelo para la vista del formulario
            var modelo = new SolicitudTutoriaRespuestaDto
            {
                IdTutor = idTutor,
                IdTutorado= idTutorado.Value,
                IdMateria = idMateria,
                NombreMateria = nombreMateria 
                //Fecha = DateTime.Today 
            };

            return View("FormSolicitudTutoria", modelo);
        }

        //RANKING DE TUTORES
        [HttpGet]
        [ValidarRol(1)]
        public async Task<IActionResult> RankingTutores()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                var (ok, msg, ranking) = await _tutoradoService.ObtenerRankingTutoresAsync(token);

                if (!ok)
                {
                    TempData["Error"] = msg;
                    return View(new List<RankingTutorDto>());
                }

               
                return View(ranking);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado: " + ex.Message;
                return View(new List<RankingTutorDto>());
            }
        }
        
        [HttpGet]
        [ValidarRol(1)]
        public async Task<IActionResult> PerfilTutor(int idTutor)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                var modelo = await _tutoradoService.ObtenerPerfilConComentariosAsync(idTutor, token);

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el perfil del tutor: " + ex.Message;
                return RedirectToAction("BusquedaTutores");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(1)]
        public async Task<IActionResult> AgregarValoracion(CrearComentarioDto dto)
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

                // Completa el DTO
                dto.IdTutorado = idTutorado.Value;
                dto.IdEstado = 1; // activo

                var mensaje = await _tutoradoService.CrearComentarioAsync(dto, token);
                TempData["Success"] = mensaje;

                // Redirige al perfil del tutor valorado
                return RedirectToAction("PerfilTutor", new { id = dto.IdTutor });
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("PerfilTutor", new { id = dto.IdTutor });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el comentario: " + ex.Message;
                return RedirectToAction("PerfilTutor", new { id = dto.IdTutor });
            }
        }
      


        }
}
