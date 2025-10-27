using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduConnect_Front.Controllers
{
    public class TutorController : Controller
    {

        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly GeneralService _generalService = new GeneralService();
        private readonly TutorService _tutorService;
        private readonly AdministradorService _administradorService = new AdministradorService();

        public TutorController(TutorService tutorService)
        {
            _tutorService = tutorService;
        }
        //[HttpGet]
        //public IActionResult PanelTutor()
        //{
        //    var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
        //    if (usuario == null)
        //    {
        //        // No hay sesión -> volver a iniciar sesión
        //        return RedirectToAction("IniciarSesion", "General");
        //    }

        //    return View(usuario); // Pasa el DTO completo a la vista
        //}
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> PanelTutor()
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

        //[HttpGet]
        //[ValidarRol(2)]
        //public async Task<IActionResult> SolicitudesTutorias(int page = 1, int pageSize = 4)
        //{
        //    var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
        //    var token = HttpContext.Session.GetString("Token");

        //    // Llamada al servicio con filtros vacíos (0 y null)
        //    var (ok, msg, solicitudes) =
        //        await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, 0, 0, token);

        //    if (!ok)
        //    {
        //        TempData["Error"] = msg;
        //        return View(new List<SolicitudTutorDto>());
        //    }
        //    // 🔹 Paginación
        //    int totalRegistros = solicitudes.Count;
        //    int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

        //    var solicitudesPaginadas = solicitudes
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    ViewBag.PaginaActual = page;
        //    ViewBag.TotalPaginas = totalPaginas;

        //    return View(solicitudes);
        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidarRol(2)]
        //public async Task<IActionResult> SolicitudesTutoriasEnviar(int idMateria, int idModalidad)
        //{
        //    var token = HttpContext.Session.GetString("Token");
        //    var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;

        //    var (ok, msg, solicitudes) =
        //        await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, idMateria, idModalidad, token);

        //    if (!ok)
        //    {
        //        TempData["Error"] = msg;
        //        return View(new List<SolicitudTutorDto>());
        //    }



        //    return View(solicitudes);
        //}
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> SolicitudesTutorias(int page = 1, int pageSize = 4, int idMateria = 0, int idModalidad = 0)
        {
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            if (idTutor == 0 || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró la sesión del usuario.";
                return RedirectToAction("IniciarSesion", "General");
            }

            // 🔹 Llamada al servicio (con filtros si los hay)
            var (ok, msg, solicitudes) = await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, idMateria, idModalidad, token);

            if (!ok || solicitudes == null)
            {
                TempData["Error"] = msg;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public IActionResult SolicitudesTutoriasEnviar(int idMateria, int idModalidad)
        {
            // Redirige al método GET, pero conservando los filtros como parámetros
            return RedirectToAction("SolicitudesTutorias", new { idMateria, idModalidad });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> AceptarSolicitud(int idTutoria)
        {
            var token = HttpContext.Session.GetString("Token");
            var (ok, msg) = await _tutorService.AceptarSolicitudTutoriaAsync(idTutoria, token);

            if (!ok)
            {
                TempData["Error"] = msg;
                return RedirectToAction("SolicitudesTutorias");
            }

            TempData["Success"] = msg;
            return RedirectToAction("SolicitudesTutorias");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> RechazarSolicitud(int idTutoria)
        {
            var token = HttpContext.Session.GetString("Token");
            var (ok, msg) = await _tutorService.RechazarSolicitudTutoriaAsync(idTutoria, token);

            if (!ok)
            {
                TempData["Error"] = msg;
                return RedirectToAction("SolicitudesTutorias");
            }

            TempData["Success"] = msg;
            return RedirectToAction("SolicitudesTutorias");
        }
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> HistorialTutor(List<int>? idEstados)
        {
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            var (ok, msg, historial) = await _tutorService.ObtenerHistorialTutorAsync(idTutor, idEstados, token);

            if (!ok)
            {
                TempData["Error"] = msg;
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
                var carreras = await _generalService.ObtenerCarrerasAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> EditarTutor(EditarPerfilDto perfil)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                var mensaje = await _tutorService.ActualizarPerfilTutorAsync(perfil, token);

                TempData["Success"] = mensaje;
                //Redirige al GET de esta acción (para evitar reenvío del formulario)
                return RedirectToAction("EditarTutor");

               

               
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("EditarTutor");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> RegistrarMaterias()
        //{
        //    var token = HttpContext.Session.GetString("Token");
        //    var idTutor = HttpContext.Session.GetInt32("IdUsu");

        //    if (string.IsNullOrEmpty(token) || idTutor == null)
        //    {
        //        TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
        //        return RedirectToAction("IniciarSesion", "General");
        //    }

        //    var (ok, msg, materias) = await _tutorService.ObtenerMateriasPorTutorAsync(idTutor.Value, token);

        //    if (!ok)
        //    {
        //        TempData["Error"] = msg;
        //        return RedirectToAction("PanelTutor", "Tutor");
        //    }

        //    return View(materias);
        //}
        [HttpGet]
        [ValidarRol(2)]
        public async Task<IActionResult> RegistrarMaterias()
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutor = HttpContext.Session.GetInt32("IdUsu");

            if (string.IsNullOrEmpty(token) || idTutor == null)
            {
                TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (ok, msg, materias) = await _tutorService.ObtenerMateriasPorTutorAsync(idTutor.Value, token);

            if (!ok)
            {
                TempData["Error"] = msg;
                return RedirectToAction("PanelTutor", "Tutor");
            }

            return View(materias);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(2)]
        public async Task<IActionResult> GuardarMaterias(int[] MateriasSeleccionadas)
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutor = HttpContext.Session.GetInt32("IdUsu");

            if (string.IsNullOrEmpty(token) || idTutor == null)
            {
                TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            if (MateriasSeleccionadas == null || MateriasSeleccionadas.Length == 0)
            {
                TempData["Error"] = "Debes seleccionar al menos una materia.";
                return RedirectToAction("RegistrarMaterias");
            }

            var (ok, msg) = await _tutorService.RegistrarMateriasAsync(idTutor.Value, MateriasSeleccionadas, token);

            TempData[ok ? "Success" : "Error"] = msg;
            return RedirectToAction("PanelTutor");
        }






    }
}
