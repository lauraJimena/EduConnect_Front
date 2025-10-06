using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class AdministradorService
    {
        private readonly API_Service _api = new API_Service();


        public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Items)>
                ConsultarUsuarios(CancellationToken ct = default)
            {
               
                return await _api.ConsultarUsuariosAsync(ct);
            }


        public async Task<(bool Ok, string Msg)> RegistrarUsuario(CrearUsuarioDto dto, CancellationToken ct = default)
        {
            return await _api.RegistrarUsuarioAdminAsync(dto, ct);
        }

        public async Task<(bool Ok, string Msg, ActualizarUsuarioDto? Usuario)> ObtenerUsuarioPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _api.ObtenerUsuarioPorIdAsync(id, ct);
        }
       

        public async Task<(bool Ok, string Msg)> ActualizarUsuarioAsync(ActualizarUsuarioDto dto, CancellationToken ct = default)
        {
            return await _api.ActualizarUsuarioAsync(dto, ct);
        }

        public async Task<(bool Ok, string Msg)> EliminarUsuarioAsync(int idUsuario, CancellationToken ct = default)
        {
            return await _api.EliminarUsuarioAsync(idUsuario, ct);
        }


    }
}
