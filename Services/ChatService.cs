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
    }
}
