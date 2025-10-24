using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class AdministradorService
    {
        private readonly API_Service _api = new API_Service();

        public async Task<(bool Ok, string Msg)> RegistrarUsuario(CrearUsuarioDto dto, string token, CancellationToken ct = default)
        {
            return await _api.RegistrarUsuarioAdminAsync(dto, token, ct);
        }

        public async Task<(bool Ok, string Msg, ActualizarUsuarioDto? Usuario)> ObtenerUsuarioPorIdPerfil(int id, string token)
        {
            return await _api.ObtenerUsuarioPorIdPerfil(id, token);
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
        // 🔹 Método que llama al ApiService
        public async Task<(bool Ok, string Msg, List<ReporteTutorDto> Reporte)> ObtenerReporteTutoresAsync(string token)
        {
            try
            {
                var (ok, msg, reporte) = await _api.ObtenerReporteTutoresAsync(token);

                if (!ok)
                    return (false, msg, new List<ReporteTutorDto>());

                return (true, msg, reporte);
            }
            catch (Exception ex)
            {
                return (false, "Error al obtener reporte: " + ex.Message, new List<ReporteTutorDto>());
            }
        }
        public async Task<List<ReporteTutoradoDto>> ObtenerReporteTutoradosActivosAsync(string token)
        {
            return await _api.ObtenerReporteTutoradosActivosAsync(token);
        }


    }
}
