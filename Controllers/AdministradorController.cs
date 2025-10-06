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
        public async Task<IActionResult> EditarUsuario(int id, CancellationToken ct)
        {
            var (ok, msg, usuario) = await _administradorService.ObtenerUsuarioPorIdAsync(id, ct);

            if (!ok || usuario == null)
            {
                ModelState.AddModelError(string.Empty, msg ?? "No se pudo obtener el usuario");
                return RedirectToAction("ConsultarUsuarios");
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(ActualizarUsuarioDto dto, CancellationToken ct)
        {
            var (ok, msg) = await _administradorService.ActualizarUsuarioAsync(dto, ct);

            if (ok)
            {
                TempData["UpdateSuccess"] = msg;
                return RedirectToAction("ConsultarUsuarios");
            }

            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarUsuarios(CancellationToken ct)
        {
            var (ok, msg, items) = await _administradorService.ConsultarUsuarios(ct);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, msg);
                items = new List<ListadoUsuariosDto>();
            }

            return View(items);
        }
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int idUsuario, CancellationToken ct)
        {
            var (ok, msg) = await _administradorService.EliminarUsuarioAsync(idUsuario, ct);

            if (ok)
                return Ok(msg);
            else
                return BadRequest(msg);
        }



    }
}
