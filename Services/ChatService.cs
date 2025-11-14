using EduConnect_Front.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduConnect_Front.Services
{
    public class ChatService
    {
        private readonly API_Service _apiService;

        public ChatService()
        {
            _apiService = new API_Service();
        }
        public async Task<(bool Success, string Message, List<ObtenerChatDto>? Chats)> ObtenerChats(int idUsuario, string token, CancellationToken ct = default) 
        { 
            try 
            {
                var chats = await _apiService.ObtenerChatsAsync(idUsuario, token, ct);
                return (true, "Chats obtenidos correctamente", chats);
            }
            catch (Exception ex) 
            { 
                return (false, ex.Message, null); 
            } 
        }

        public async Task<(bool Success, string Message, List<ObtenerMensajeDto>? Mensajes)> ObtenerMensajes(int idChat, string token, CancellationToken ct = default) 
        {
            try { 
                var mensajes = await _apiService.ObtenerMensajesAsync(idChat, token, ct);
                return (true, "Mensajes obtenidos correctamente", mensajes);
            } 
            catch (Exception ex) 
            {
                return (false, ex.Message, null); 
            } 
        }

        public async Task<(bool Success, string Message)> EnviarMensaje(CrearMensajeDto mensaje, string token, CancellationToken ct = default)
        {
            try
            {
                var (success, message) = await _apiService.EnviarMensajeAsync(mensaje, token, ct);
                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
