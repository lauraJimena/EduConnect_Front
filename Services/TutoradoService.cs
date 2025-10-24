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


     
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Datos)> ObtenerHistorialAsync(
    int idTutorado,
    string token,                          
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
        // TutoradoService
        public async Task<EditarPerfilDto?> ObtenerUsuarioParaEditarAsync(int idUsuario, string token, CancellationToken ct = default)
        {
            var (ok, msg, usuario) = await _api.ObtenerUsuarioPorIdAsync(idUsuario, token, ct);
            if (!ok || usuario == null) return null;

            // Mapeo a lo que la vista de edición necesita
            return new EditarPerfilDto
            {
                IdUsu = usuario.IdUsu,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                TelUsu = usuario.TelUsu,
                IdCarrera = (usuario.IdCarrera.HasValue && usuario.IdCarrera.Value > 0) ? usuario.IdCarrera : null,
                IdSemestre = (usuario.IdSemestre.HasValue && usuario.IdSemestre.Value > 0) ? usuario.IdSemestre : null,
                NumIdent = usuario.NumIdent,
                IdTipoIdent = usuario.IdTipoIdent,
                Avatar = usuario.Avatar // <-- IMPORTANTE
            };
        }

        // (Opcional) si en algún otro lugar necesitas el raw:
        public async Task<(bool Ok, string Msg, ObtenerUsuarioDto? Data)> ObtenerUsuarioPorIdRawAsync(
            int idUsuario, string token, CancellationToken ct = default)
            => await _api.ObtenerUsuarioPorIdAsync(idUsuario, token, ct);

        public async Task<List<SolicitudTutoriaDto>> ObtenerSolicitudesTutoriasAsync(
        FiltroSolicitudesDto filtro, string token, CancellationToken ct = default)
        {
            return await _api.ObtenerSolicitudesTutoriasAsync(filtro, token, ct);
        }

        public async Task<(bool Success, string Message, int IdTutoria)> CrearSolicitudTutoriaAsync(
            SolicitudTutoriaRespuestaDto solicitud,
            string token)
        {
            try
            {
                var result = await _api.CrearSolicitudTutoriaAsync(solicitud, token);
                return (result.Success, result.Message, result.IdTutoria);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear tutoría: {ex.Message}", 0);
            }
        }

        public async Task<(bool Ok, string Msg, List<ObtenerTutorDto> Tutores)> BuscarTutoresAsync(BuscarTutorDto filtros, string token)
        {
            try
            {
                var tutores = await _api.BuscarTutoresAsync(filtros, token);

                if (tutores == null || !tutores.Any())
                    return (false, "No se encontraron tutores con los filtros aplicados.", new List<ObtenerTutorDto>());

                return (true, $"Se encontraron {tutores.Count} tutores.", tutores);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener tutores: {ex.Message}", new List<ObtenerTutorDto>());
            }
        }
        public async Task<(bool Ok, string Msg, List<RankingTutorDto> Data)> ObtenerRankingTutoresAsync(string token)
        {
            return await _api.ObtenerRankingTutoresAsync(token);
        }
        public async Task<(bool Ok, string Msg, PerfilTutorDto? Data)> ObtenerPerfilTutorAsync(int idTutor, string token)
        {
            return await _api.ObtenerPerfilTutorAsync(idTutor, token);
        }

        // TutoradoService.cs
        public async Task<EditarPerfilDto?> ObtenerTutoradoParaEditarAsync(int idUsuario, string token)
        {
            var usuario = await _api.ObtenerTutoradoPorIdAsync(idUsuario, token);

            if (usuario == null)
                return null;

            // Mapeo de ObtenerUsuarioDto → EditarPerfilDto
            return new EditarPerfilDto
            {
                IdUsu = usuario.IdUsu,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                TelUsu = usuario.TelUsu,
                IdCarrera = usuario.IdCarrera > 0 ? usuario.IdCarrera : null,
                IdSemestre = usuario.IdSemestre > 0 ? usuario.IdSemestre : null,
                NumIdent = usuario.NumIdent,
                IdTipoIdent = usuario.IdTipoIdent,
                Avatar = usuario.Avatar 
            };
        }
       
        public async Task<string> ActualizarPerfilTutorado(EditarPerfilDto perfil, string token)
        {
            try
            {
                return await _api.ActualizarPerfilAsync(perfil, token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el perfil: " + ex.Message);
            }
        }
        public async Task<PerfilTutorComentariosDto> ObtenerPerfilConComentariosAsync(int idTutor, string token)
        {
            var perfilResult = await _api.ObtenerPerfilTutorAsync(idTutor, token);
            var comentarios = await _api.ObtenerComentariosPorTutorAsync(idTutor, token);

            if (!perfilResult.Ok || perfilResult.Data == null)
                throw new Exception($"Error al obtener el perfil del tutor: {perfilResult.Msg}");

            return new PerfilTutorComentariosDto
            {
                Perfil = perfilResult.Data, 
                Comentarios = comentarios?.ToList() ?? new List<ComentarioTutorInfoDto>()
            };
        }
        public async Task<string> CrearComentarioAsync(CrearComentarioDto dto, string token)
        {
            if (dto.Calificacion < 1 || dto.Calificacion > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5 estrellas.");

            if (string.IsNullOrWhiteSpace(dto.Texto))
                throw new ArgumentException("El comentario no puede estar vacío.");

            if (dto.Texto.Length < 10)
                throw new ArgumentException("El comentario debe tener al menos 10 caracteres.");

            return await _api.CrearComentarioAsync(dto, token);
        }
        public async Task<bool> EnviarCorreoConfirmacionTutoriaAsync(string token, int idTutoria)
        {
            return await _api.EnviarConfirmacionTutoriaAsync(token, idTutoria);
        }




    }

}

