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
            //var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            string token = HttpContext.Session.GetString("Token") ?? "";
            if (token == null)
            {
                return Unauthorized("El usuario no está autenticado.");
            }
            int idUsuario = (int)HttpContext.Session.GetInt32("IdUsu");
            //int idUsuario = usuario.IdUsu;
           

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
            //var usuario = HttpContext.Session.GetObject<ObtenerUsuarioDto>("Usuario");
            string token = HttpContext.Session.GetString("Token") ?? "";
            if (token == null) 
            { 
                return Unauthorized("El usuario no está autenticado.");
            } 
            
            
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
            string token = HttpContext.Session.GetString("Token") ?? "";
            if (token == null) return Unauthorized();    
            var (success, message) = await _chatService.EnviarMensaje(nuevoMensaje, token);
            return success ? Ok() : BadRequest(message);
        }
    }

}
