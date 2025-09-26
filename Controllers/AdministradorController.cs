using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class AdministradorController : Controller
    {
        // GET: TutoradoController
        public ActionResult ConsultarUsuarios()
        {
            return View();
        }

    }
}
