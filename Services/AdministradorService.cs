using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class AdministradorService
    {
        private readonly API_Service _api = new API_Service();


        //public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Items)>
        //        ConsultarUsuarios(CancellationToken ct = default)
        //    {

        //        return await _api.ConsultarUsuariosAsync(ct);
        //    }
        //public async Task<List<ListadoUsuariosDto>> ObtenerUsuariosAsync(int? idRol = null, int? idEstado = null, string? numIdent = null)
        //{
        //    return await _api.ObtenerUsuariosAsync(idRol, idEstado, numIdent);
        //}

        public async Task<(bool Ok, string Msg)> RegistrarUsuario(CrearUsuarioDto dto, CancellationToken ct = default)
        {
            return await _api.RegistrarUsuarioAdminAsync(dto, ct);
        }

        public async Task<(bool Ok, string Msg, ActualizarUsuarioDto? Usuario)> ObtenerUsuarioPorIdPerfil(int id, string token, CancellationToken ct = default)
        {
            return await _api.ObtenerUsuarioPorIdPerfil(id, token, ct);
        }
        public async Task<(bool Ok, string Msg, ObtenerUsuarioDto? Usuario)> ObtenerUsuarioPorIdAsync(int id, string token, CancellationToken ct = default)
        {
            return await _api.ObtenerUsuarioPorIdAsync(id, token, ct);

        }    


        public async Task<(bool Ok, string Msg)> ActualizarUsuarioAsync(ActualizarUsuarioDto dto, string token, CancellationToken ct = default)
        {
            return await _api.ActualizarUsuarioAsync(dto, token, ct);
        }

        public async Task<(bool Ok, string Msg)> EliminarUsuarioAsync(int idUsuario, string token, CancellationToken ct = default)
        {
            return await _api.EliminarUsuarioAsync(idUsuario, token, ct);
        }
        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            return await _api.ObtenerCarrerasAsync();
        }
        
        public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Usuarios)>
            ObtenerUsuariosAsync(string token, int? idRol = null, int? idEstado = null, string? numIdent = null)
        {
            return await _api.ObtenerUsuariosAsync(token, idRol, idEstado, numIdent);
        }


    }
}
