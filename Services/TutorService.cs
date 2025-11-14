using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class TutorService
    {
        //private readonly API_Service _apiService;

        private readonly API_Service _apiService = new API_Service();


        public async Task<(bool Ok, string Msg, List<SolicitudTutorDto>? Solicitudes)>
            ObtenerSolicitudesTutoriasAsync(int idTutor, int idMateria, int idModalidad, string token)
        {
            var filtro = new FiltroSolicitudesTutorDto
            {
                IdTutor = idTutor,
                IdMateria = idMateria,
                IdModalidad= idModalidad
            };

            return await _apiService.ObtenerSolicitudesTutoriasAsync(filtro, token);
        }
        public async Task<(bool Ok, string Msg)> AceptarSolicitudTutoriaAsync(int idTutoria, string token)
        {
            return await _apiService.AceptarSolicitudTutoriaAsync(idTutoria, token);
        }
        public async Task<(bool Ok, string Msg)> RechazarSolicitudTutoriaAsync(int idTutoria, string token)
        {
            return await _apiService.RechazarSolicitudTutoriaAsync(idTutoria, token);
        }
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Data)>
    ObtenerHistorialTutorAsync(int idTutor, List<int>? estados, string token)
        {
            return await _apiService.ObtenerHistorialTutorAsync(idTutor, estados, token);
        }

        public async Task<string> ActualizarPerfilTutorAsync(EditarPerfilDto perfil, string token)
        {
            return await _apiService.ActualizarPerfilTutor(perfil, token);
        }
        // TutoradoService.cs
        public async Task<EditarPerfilDto?> ObtenerTutoradoParaEditarAsync(int idUsuario, string token)
        {
            var usuario = await _apiService.ObtenerTutoradoPorIdAsync(idUsuario, token);

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
        public async Task<(bool TieneMaterias, string Msg)> ValidarMateriasTutorAsync(int idTutor, string token)
        {
            var result = await _apiService.ValidarMateriasTutorAsync(idTutor, token);

            if (!result.Ok)
                throw new Exception(result.Msg);

            return (result.TieneMaterias, result.Msg);
        }

        // ✅ Obtener materias según carrera y semestre del tutor
        public async Task<(bool Ok, string Msg, List<MateriaDto>? Data)> ObtenerMateriasPorTutorAsync(int idTutor, string token)
        {
            var result = await _apiService.ObtenerMateriasPorTutorAsync(idTutor, token);
            if (!result.Ok)
                throw new Exception(result.Msg);

            return (result.Ok, result.Msg, result.Data);
        }

        // ✅ Registrar materias seleccionadas
        public async Task<(bool Ok, string Msg)> RegistrarMateriasAsync(int idTutor, int[] materias, string token)
        {
            var result = await _apiService.RegistrarMateriasTutorAsync(idTutor, materias, token);

            if (!result.Ok)
                throw new Exception(result.Msg);

            return (result.Ok, result.Msg);
        }
        public async Task<(bool Ok, string Msg, List<ComentarioTutorDto>? Data)> ObtenerComentariosTutorAsync(
    int idTutor, int? calificacion, int? ordenFecha, string token)
        {
            var filtro = new FiltrosComentariosTutorDto
            {
                IdTutor = idTutor,
                Calificacion = calificacion,
                OrdenFecha = ordenFecha
            };

            var resultado = await _apiService.ObtenerComentariosTutorAsync(filtro, token);

            if (resultado == null)
                return (false, "Error al consultar comentarios.", null);

            return (true, "OK", resultado);
        }





    }
}
