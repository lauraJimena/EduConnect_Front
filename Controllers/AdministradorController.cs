using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class AdministradorController : Controller
    {
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

        public ActionResult PanelAdministrador()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id, string token, CancellationToken ct)
        {

            var (ok, msg, usuario) = await _administradorService.ObtenerUsuarioPorIdPerfil(id, token, ct);
            

            if (!ok || usuario == null)
            {
                ModelState.AddModelError(string.Empty, msg ?? "No se pudo obtener el usuario");
                return RedirectToAction("ConsultarUsuarios");
            }
            GeneralService _generalService = new GeneralService();
            var tipoIdent = await _generalService.ObtenerTipoIdentAsync();
            ViewBag.TipoIdent = tipoIdent;
            // Cargar carreras para el dropdown
            var carreras = await _administradorService.ObtenerCarrerasAsync();
            ViewBag.Carreras = carreras;

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(ActualizarUsuarioDto dto, CancellationToken ct)
        {
            var (ok, msg) = await _administradorService.ActualizarUsuarioAsync(dto, ct);
            var carreras = await _administradorService.ObtenerCarrerasAsync();

            ViewBag.Carreras = carreras;
            if (ok)
            {
                TempData["UpdateSuccess"] = msg;
                return RedirectToAction("ConsultarUsuarios");
            }

            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
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
            // Llamar al servicio para traer usuarios con filtros
            var usuarios = await _administradorService.ObtenerUsuariosAsync(idRol, idEstado, numIdent);

            //// Traer roles y estados para llenar dropdowns
            //ViewBag.Roles = await _administradorService.ObtenerRolesAsync();
            //ViewBag.Estados = await _administradorService.ObtenerEstadosAsync();

            //// Mantener valores seleccionados en los filtros
            //ViewBag.FiltroRol = idRol;
            //ViewBag.FiltroEstado = idEstado;
            //ViewBag.FiltroNumIdent = numIdent;

            return View(usuarios);
        }



    }
}
