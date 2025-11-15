using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduConnect_Front.Controllers
{
    public class TutorController : Controller
    {
        private const string AccionSolicitudesTutorias = "SolicitudesTutorias";
        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly GeneralService _generalService = new GeneralService();
        private readonly TutorService _tutorService;
        private readonly AdministradorService _administradorService = new AdministradorService();
        public const string SessionExpiredMessage = "Sesión expirada. Inicia sesión nuevamente.";
        public const string Error = "Errror";
        public TutorController(TutorService tutorService)
        {
            _tutorService = tutorService;
        }
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> PanelTutor()
        {

            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = SessionExpiredMessage;
                    return RedirectToAction("IniciarSesion", "General");
                }              
                var idUsuario = HttpContext.Session.GetInt32("IdUsu");

                if (idUsuario == null)
                {
                    TempData[Error] = "No se encontró información del usuario actual.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                //Volver a consultar al backend por los datos actualizados
                var usuario = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsuario.Value, token);

                if (usuario == null)
                {
                    TempData[Error] = "No se pudo cargar el perfil del tutorado.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData[Error] = ex.Message;
                return RedirectToAction("IniciarSesion", "General");
            }

        }
        #pragma warning disable S5122 // ModelState.IsValid should be checked in controller actions
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> SolicitudesTutorias(int page = 1, int pageSize = 4, int idMateria = 0, int idModalidad = 0)
        {
            if (!ModelState.IsValid)
            {
                TempData[Error] = "Solicitud no válida.";
                return RedirectToAction(AccionSolicitudesTutorias);
            }

            // ModelState.IsValid no aplica en este método: no se recibe un modelo complejo, solo parámetros simples de filtro/paginación.
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;
            if (idMateria < 0 || idModalidad < 0)
            {
                TempData[Error] = "Filtros inválidos.";
                return RedirectToAction(AccionSolicitudesTutorias);
            }

            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            if (idTutor == 0 || string.IsNullOrEmpty(token))
            {
                TempData[Error] = "No se encontró la sesión del usuario.";
                return RedirectToAction("IniciarSesion", "General");
            }

            // 🔹 Llamada al servicio (con filtros si los hay)
            var (ok, msg, solicitudes) = await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, idMateria, idModalidad, token);

            if (!ok || solicitudes == null)
            {
                TempData[Error] = msg;
                return View(new List<SolicitudTutorDto>());
            }

            // 🔹 Paginación
            int totalRegistros = solicitudes.Count;
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var solicitudesPaginadas = solicitudes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 Guardamos datos en ViewBag para conservar filtros al paginar
            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.FiltroMateria = idMateria;
            ViewBag.FiltroModalidad = idModalidad;

            return View(solicitudesPaginadas);
        }
        #pragma warning restore S5122

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public IActionResult SolicitudesTutoriasEnviar(int idMateria, int idModalidad)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(idMateria);
            }
            // Redirige al método GET, pero conservando los filtros como parámetros
            return RedirectToAction(AccionSolicitudesTutorias, new { idMateria, idModalidad });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> AceptarSolicitud(int idTutoria)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(idTutoria);
            }
            var token = HttpContext.Session.GetString("Token");
            var (ok, msg) = await _tutorService.AceptarSolicitudTutoriaAsync(idTutoria, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return RedirectToAction(AccionSolicitudesTutorias);
            }

            TempData["Exito"] = msg;
            return RedirectToAction(AccionSolicitudesTutorias);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> RechazarSolicitud(int idTutoria)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(idTutoria);
            }
            var token = HttpContext.Session.GetString("Token");
            var (ok, msg) = await _tutorService.RechazarSolicitudTutoriaAsync(idTutoria, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return RedirectToAction(AccionSolicitudesTutorias);
            }

            TempData["Exito"] = msg;
            return RedirectToAction(AccionSolicitudesTutorias);
        }
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> HistorialTutor(List<int>? idEstados)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View();
            }
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            var (ok, msg, historial) = await _tutorService.ObtenerHistorialTutorAsync(idTutor, idEstados, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return View(new List<HistorialTutoriaDto>());
            }

            TempData["Success"] = msg;
            return View(historial);
        }
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> EditarTutor()
        {

            try
            {
                var token = HttpContext.Session.GetString("Token");
                var idUsu = HttpContext.Session.GetInt32("IdUsu");


                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                // 🔹 Llama al service que ya devuelve EditarPerfilDto
                var modelo = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsu.Value, token);

                if (modelo == null)
                {
                    TempData[Error] = "Usuario no encontrado.";
                    return RedirectToAction("ConsultarUsuarios");
                }
               

                var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
                var carreras = await _generalService.ObtenerCarrerasAsync();
                ViewBag.TipoIdent = tipoIdent;
                ViewBag.Carreras = carreras;

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData[Error] = ex.Message;
                return RedirectToAction("ConsultarUsuarios");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> EditarTutor(EditarPerfilDto perfil)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(perfil);
            }
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = SessionExpiredMessage;
                    return RedirectToAction("IniciarSesion", "General");
                }
                var mensaje = await _tutorService.ActualizarPerfilTutorAsync(perfil, token);

                TempData["Success"] = mensaje;
               
                HttpContext.Session.SetString("AvatarUrl", perfil.Avatar);
                HttpContext.Session.SetString("UsuarioNombre", perfil.Nombre);
                return RedirectToAction("EditarTutor");

               

               
            }
            catch (Exception ex)
            {
                TempData[Error] = ex.Message;
                return RedirectToAction("EditarTutor");
            }
        }

        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> RegistrarMaterias()
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutor = HttpContext.Session.GetInt32("IdUsu");

            if (string.IsNullOrEmpty(token) || idTutor == null)
            {
                TempData[Error] = SessionExpiredMessage;
                return RedirectToAction("IniciarSesion", "General");
            }

            var (ok, msg, materias) = await _tutorService.ObtenerMateriasPorTutorAsync(idTutor.Value, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return RedirectToAction("PanelTutor", "Tutor");
            }

            return View(materias);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> GuardarMaterias(int[] MateriasSeleccionadas)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(MateriasSeleccionadas);
            }
            var token = HttpContext.Session.GetString("Token");
            var idTutor = HttpContext.Session.GetInt32("IdUsu");

            if (string.IsNullOrEmpty(token) || idTutor == null)
            {
                TempData[Error] = SessionExpiredMessage;
                return RedirectToAction("IniciarSesion", "General");
            }

            if (MateriasSeleccionadas == null || MateriasSeleccionadas.Length == 0)
            {
                TempData[Error] = "Debes seleccionar al menos una materia.";
                return RedirectToAction("RegistrarMaterias");
            }

            var (ok, msg) = await _tutorService.RegistrarMateriasAsync(idTutor.Value, MateriasSeleccionadas, token);

            TempData[ok ? "Success" : "Error"] = msg;
            return RedirectToAction("PanelTutor");
        }
        [HttpGet]
        [ValidarRol(2)] // solo los tutores
        public async Task<IActionResult> ComentariosTutor()
        {
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            if (idTutor == 0 || string.IsNullOrEmpty(token))
            {
                TempData[Error] = "Sesión no válida.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (ok, msg, data) = await _tutorService.ObtenerComentariosTutorAsync(idTutor, null, 1, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return View(new List<ComentarioTutorDto>());
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> ComentariosTutorFiltrar(int? calificacion, int? ordenFecha)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View();
            }
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            var (ok, msg, data) = await _tutorService.ObtenerComentariosTutorAsync(idTutor, calificacion, ordenFecha, token);

            if (!ok)
            {
                TempData[Error] = msg;
                return View("ComentariosTutor", new List<ComentarioTutorDto>());
            }

            return View("ComentariosTutor", data);
        }







    }
}
