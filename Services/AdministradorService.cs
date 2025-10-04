using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class AdministradorService
    {

            public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Items)>
                ConsultarUsuarios(CancellationToken ct = default)
            {
                var api = new API_Service();
                return await api.ConsultarUsuariosAsync(ct);
            }

        
    }
}
