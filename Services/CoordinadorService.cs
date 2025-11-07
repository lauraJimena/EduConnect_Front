using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class CoordinadorService
    {
        private readonly API_Service _api = new API_Service();

        public async Task<List<TutoriaConsultaDto>> ConsultarTutoriasAsync(
    string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha, string token)
        {
            return await _api.ConsultarTutoriasCoordAsync(carrera, semestre, materia, idEstado, ordenFecha, token);
        }

        public async Task<ReporteDemandaAcademicaDto?> ObtenerReporteDemandaAcademicaAsync(string? token)
        {
            return await _api.ObtenerReporteDemandaAcademicaAsync(token);
        }


        public async Task<(ReporteGestionAdministrativaDto? Totales, List<ReporteDesempenoTutorDto>? Desempeno)>
               ObtenerReporteCombinadoAsync(string token)
        {
            return await _api.ObtenerReporteCombinadoAsync(token);
        }
        public async Task<List<ListaComentariosDto>> ObtenerComentariosAsync(string token)
        {
            try
            {
                return await _api.ObtenerComentariosAsync(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de comentarios: " + ex.Message);
            }
        }
        public async Task<string> InactivarComentarioAsync(int idComentario, string token)
        {
            if (idComentario <= 0)
                throw new ArgumentException("El Id del comentario no es válido.");

            try
            {
                return await _api.InactivarComentarioAsync(idComentario, token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar inactivar el comentario: " + ex.Message);
            }
        }
    }
}
