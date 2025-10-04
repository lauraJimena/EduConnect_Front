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
        public ActionResult RegistrarUsuarios()
        {
            return View();
        }

        public ActionResult PanelAdministrador()
        {
            return View();
        }
        public ActionResult EditarUsuarios()
        {
            return View();
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


    }
}
