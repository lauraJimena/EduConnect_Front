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
        private readonly ChatService _chatService = new ChatService();

        public async Task<IActionResult> Lista()
        {
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");

            if (usuario == null)
            {
                return Unauthorized("El usuario no está autenticado.");
            }

            int idUsuario = usuario.IdUsu;
            string token = HttpContext.Session.GetString("Token") ?? "";

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token no encontrado en sesión.");
            }

            var (success, message, chats) = await _chatService.ObtenerChats(idUsuario, token);

            if (!success || chats == null)
            {
                ViewBag.Error = message;
                return PartialView("_ListaChats", new List<ObtenerChatDto>());
            }

            return PartialView("_ListaChats", chats);
        }


        public async Task<IActionResult> Mensajes(int idChat, CancellationToken cancellationToken) 
        { 
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            if (usuario == null) 
            { 
                return Unauthorized("El usuario no está autenticado.");
            } 
            int idUsuario = usuario.IdUsu; 
            string token = HttpContext.Session.GetString("Token") ?? "";
            if (string.IsNullOrEmpty(token)) 
            { 
                return Unauthorized("Token no encontrado en sesión."); 
            } 
            try 
            {
                var (success, message, mensajes) = await _chatService.ObtenerMensajes(idChat, token, cancellationToken); 
                if (!success || mensajes == null) { ViewBag.Error = message;
                    return PartialView("_ListaMensajes", new List<ObtenerMensajeDto>());
                } 
                return PartialView("_ListaMensajes", mensajes); 
            } 
            catch (Exception ex) 
            { 
                return Content("ERROR EN Mensajes(): " + ex.ToString());
            } 
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] CrearMensajeDto nuevoMensaje)
        {
            var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            if (usuario == null) return Unauthorized();

            string token = HttpContext.Session.GetString("Token") ?? "";

            var (success, message) = await _chatService.EnviarMensaje(nuevoMensaje, token);
            return success ? Ok() : BadRequest(message);
        }
    }

}
