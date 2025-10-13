using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class TutorController : Controller
    {


        private readonly TutorService _tutorService;

        public TutorController(TutorService tutorService)
        {
            _tutorService = tutorService;
        }
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
        [HttpGet]
        public async Task<IActionResult> SolicitudesTutorias()
        {
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;
            var token = HttpContext.Session.GetString("Token");

            // Llamada al servicio con filtros vacíos (0 y null)
            var (ok, msg, solicitudes) =
                await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, 0, 0, token);

            if (!ok)
            {
                TempData["Error"] = msg;
                return View(new List<SolicitudTutorDto>());
            }

            
            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitudesTutorias(int idMateria, int idModalidad)
        {
            var token = HttpContext.Session.GetString("Token");
            var idTutor = HttpContext.Session.GetInt32("IdUsu") ?? 0;

            var (ok, msg, solicitudes) =
                await _tutorService.ObtenerSolicitudesTutoriasAsync(idTutor, idMateria, idModalidad, token);

            if (!ok)
            {
                TempData["Error"] = msg;
                return View(new List<SolicitudTutorDto>());
            }

            
            return View(solicitudes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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



    }
}
