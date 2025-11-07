using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace EduConnect_Front.Controllers
{
    public class CoordinadorController : Controller
    {
        private readonly CoordinadorService _coordinadorService = new CoordinadorService();
        // GET: CoordinadorController
        public ActionResult PanelCoordinador()
        {
            return View();
        }
        [HttpGet]
        [ValidarRol(4)] // Solo el coordinador puede entrar
        public async Task<IActionResult> ConsultarTutorias(
    string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            try
            {
                var tutorias = await _coordinadorService.ConsultarTutoriasAsync(
                    carrera, semestre, materia, idEstado, ordenFecha, token);

                ViewBag.Carrera = carrera;
                ViewBag.Semestre = semestre;
                ViewBag.Materia = materia;
                ViewBag.IdEstado = idEstado;
                ViewBag.OrdenFecha = ordenFecha;

                return View(tutorias);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consultar tutorías: {ex.Message}";
                return View(new List<TutoriaConsultaDto>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReporteDemandaAcademicaPdf()
        {
            var token = HttpContext.Session.GetString("Token");
            var reporte = await _coordinadorService.ObtenerReporteDemandaAcademicaAsync(token);

            if (reporte == null)
            {
                TempData["Error"] = "No hay datos para generar el PDF.";
                return RedirectToAction("PanelCoordinador");
            }

            return new ViewAsPdf("ReporteDemandaAcademica", reporte)
            {
                FileName = "Reporte_Demanda_Academica.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
        [HttpGet]
        public async Task<IActionResult> ReporteCombinadoPdf()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
                return RedirectToAction("IniciarSesion", "General");

            var (totales, desempeno) = await _coordinadorService.ObtenerReporteCombinadoAsync(token);

            if (totales == null || desempeno == null)
                return View("Error", "No se pudo generar el reporte.");

            var modelo = new
            {
                Totales = totales,
                Desempeno = desempeno
            };

            //Generar PDF directamente
            return new ViewAsPdf("ReporteCombinadoPdf", modelo)
            {
                FileName = "Reporte_Combinado.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
        public async Task<IActionResult> ListaComentarios()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var comentarios = await _coordinadorService.ObtenerComentariosAsync(token);
                return View(comentarios); 
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ComentarioTutorInfoDto>());
            }
        }
        [HttpPost]
        public async Task<IActionResult> InactivarComentario(int idComentario)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token"); // si usas sesión JWT
                var mensaje = await _coordinadorService.InactivarComentarioAsync(idComentario, token);
                TempData["MensajeExito"] = "Comentario inactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message;
            }

            
            return RedirectToAction("ListaComentarios");
        }




    }
}
