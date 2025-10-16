using EduConnect_Front.Dtos;
using System.Reflection;
using System.Text.Json;

namespace EduConnect_Front.Services
{
    public class TutoradoService
    {
        private readonly API_Service _api = new API_Service();

        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Items)>
          ObtenerHistorialAsync(int idTutorado, CancellationToken ct = default)
        {
            var api = new API_Service();
            return await api.ObtenerHistorialTutoradoAsync(idTutorado, ct);
        }
       

        // Llama a POST /Tutor/obtener con filtros (pueden ir vacíos)
        public async Task<(bool Ok, string Msg, List<ObtenerTutorDto> Items)>
            BuscarTutoresAsync(BuscarTutorDto filtros, CancellationToken ct = default)
        {
            var (ok, msg, items) = await _api.BuscarTutoresAsync(filtros, ct);
            return (ok, msg, items ?? new List<ObtenerTutorDto>());
        }

        // 🔹 Obtener historial del tutorado (solo delega)
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Datos)> ObtenerHistorialAsync(
    int idTutorado,
    string token,                            // ✅ Agregamos el token
    List<int>? idsEstado = null,
    CancellationToken ct = default)
        {
            return await _api.ObtenerHistorialTutoradoAsync(idTutorado, token, idsEstado, ct);
        }
        public async Task<string> ActualizarPerfilAsync(EditarPerfilDto perfil, string token)
        {
            return await _api.ActualizarPerfilAsync(perfil, token);
        }
        // 🔹 Método usado para el formulario de edición
        public async Task<EditarPerfilDto?> ObtenerUsuarioParaEditarAsync(int idUsuario, string token)
        {
            var usuario = await _api.ObtenerUsuarioPorIdAsync(idUsuario, token);

            if (usuario == null)
                return null;

            // 🔹 Mapeo de ObtenerUsuarioDto → EditarPerfilDto
            var modelo = new EditarPerfilDto
            {
                IdUsu = usuario.IdUsu,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                TelUsu = usuario.TelUsu,
                IdCarrera = usuario.IdCarrera > 0 ? usuario.IdCarrera : null,
                IdSemestre = usuario.IdSemestre > 0 ? usuario.IdSemestre : null,

                NumIdent = usuario.NumIdent,
                IdTipoIdent = usuario.IdTipoIdent
                // agrega aquí otros campos que uses en la vista de edición
            };

            return modelo;
        }
        public async Task<List<SolicitudTutoriaDto>> ObtenerSolicitudesTutoriasAsync(
        FiltroSolicitudesDto filtro, string token, CancellationToken ct = default)
        {
            return await _api.ObtenerSolicitudesTutoriasAsync(filtro, token, ct);
        }

        public async Task<(bool Success, string Message)> CrearSolicitudTutoriaAsync(
            SolicitudTutoriaRespuestaDto solicitud,
            string token)
        {
            return await _api.CrearSolicitudTutoriaAsync(solicitud, token);
        }
    





}
}
