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

        // GET: TutoradoController
        [HttpGet]
        public IActionResult PanelTutorado()
        {
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            if (usuario == null)
            {
                // No hay sesión volver a iniciar sesión
                return RedirectToAction("IniciarSesion", "General");
            }

            return View(usuario); // Pasa el DTO completo a la vista
        }
        public ActionResult EditarTutorado()
        {
            return View();
        }

        public ActionResult RankingTutores()
        {
            return View();
        }
        public ActionResult SolicitudesTutorias()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> HistorialTutorias(CancellationToken ct)
        {
            //Recuperar el usuario de la sesión
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            if (usuario == null)
                return RedirectToAction("IniciarSesion", "General");

            ViewBag.Usuario = usuario; // para saludo en la vista

            //Llamar al back con el idTutorado
            var (ok, msg, items) = await _tutoradoService.ObtenerHistorialAsync(usuario.IdUsu, ct: ct);
            // si tu DTO no tiene IdUsuario pero sí NumIdent, cámbialo por lo que tu API espera

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, msg);
                items = new List<HistorialTutoriaDto>();
            }

            //Devolver la vista con la lista
            return View(items);
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

    }
}
