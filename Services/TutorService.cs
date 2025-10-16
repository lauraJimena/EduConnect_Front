using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class TutorService
    {
        private readonly API_Service _apiService;


        public TutorService(API_Service apiService)
        {
            _apiService = apiService;
        }

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
        public async Task<(bool Ok, string Msg)> ActualizarPerfilTutorAsync(EditarPerfilDto perfil, string token)
        {
            return await _apiService.ActualizarPerfilTutorAsync(perfil, token);
        }


    }
}
