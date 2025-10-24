using EduConnect_Front.Dtos;

namespace EduConnect_Front.Services
{
    public class GeneralService
    {
        private readonly API_Service _apiService;

        public GeneralService()
        {
            _apiService = new API_Service();
        }

        public async Task<(bool Success, string Message)> RegistrarUsuario(CrearUsuarioDto usuario, string token, CancellationToken ct = default)
        {
            // aquí validaciones
            return await _apiService.RegistrarUsuarioAsync(usuario, token,ct);
        }
        public async Task<(bool Ok, string Msg, RespuestaInicioSesionDto? Usuario)> IniciarSesion(IniciarSesionDto dto, CancellationToken ct = default)
        {
            
            return await _apiService.IniciarSesionAsync(dto, ct);
        }
        public async Task<List<TipoIdentDto>> ObtenerTipoIdentAsync()
        {
            return await _apiService.ObtenerTipoIdentAsync();
        }
        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            return await _apiService.ObtenerCarrerasAsync();
        }
    }
}
