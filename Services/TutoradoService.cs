using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class TutoradoService
    {
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Items)>
          ObtenerHistorialAsync(int idTutorado, CancellationToken ct = default)
        {
            var api = new API_Service();
            return await api.ObtenerHistorialTutoradoAsync(idTutorado, ct);
        }
        private readonly API_Service _api = new API_Service();

        // Llama a POST /Tutor/obtener con filtros (pueden ir vacíos)
        public async Task<(bool Ok, string Msg, List<ObtenerTutorDto> Items)>
            BuscarTutoresAsync(BuscarTutorDto filtros, CancellationToken ct = default)
        {
            var (ok, msg, items) = await _api.BuscarTutoresAsync(filtros, ct);
            return (ok, msg, items ?? new List<ObtenerTutorDto>());
        }




    }
}
