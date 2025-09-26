using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_Front.Controllers
{
    public class TutoradoController : Controller
    {
        // GET: TutoradoController
        public ActionResult PanelTutorado()
        {
            return View();
        }
        public ActionResult EditarTutorado()
        {
            return View();
        }
        public ActionResult BusquedaTutores()
        {
            return View();
        }
        public ActionResult RankingTutores()
        {
            return View();
        }

    }
}
