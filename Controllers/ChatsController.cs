using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EduConnect_Front.Controllers
{
    public class ChatsController : Controller
    {
        private readonly ILogger<ChatsController> _logger;
        private readonly ChatService _chatService;

        public ChatsController(ILogger<ChatsController> logger, ChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Lista()
        {
            //_logger.LogInformation("Entrando a la acción Lista de ChatsController");
            //Console.WriteLine("Mensaje de depuración");

            var chat = new List<(string Nombre, string Materia)>
            {
                ("Juan Pérez", "Matemáticas Especiales"),
                ("Valeria Torres", "Física I"),
                ("Pedro Rojas", "Cálculo I")
            };
            /*var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");


            // Obtener idUsuario y token de la sesión, claims, etc. Aquí se usan valores de ejemplo
            int idUsuario = usuario.IdUsu; // TODO: Reemplaza por el id real del usuario
            string token = "token_de_ejemplo"; //TODO: Reemplaza por el token real

            var (success, message, chats) = await _chatService.ObtenerChats(idUsuario, token);
            if (!success || chats == null)
            {
                // Puedes manejar el error como prefieras
                ViewBag.Error = message;
                return PartialView("_ListaChats", new List<ObtenerChatDto>());
            }*/
            return PartialView("_ListaChats", chat);
        }
    }
}
