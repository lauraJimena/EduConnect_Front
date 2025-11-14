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
        public async Task<(bool Ok, string Msg)> ActualizarPasswordAsync(
        int idUsuario,
        string token,
        string nuevaPassword,
        CancellationToken ct = default)
        {
            // 🔹 Validar datos de entrada
            if (idUsuario <= 0)
                return (false, "El ID de usuario no es válido.");

            if (string.IsNullOrWhiteSpace(token))
                return (false, "No se encontró un token de autenticación válido.");

            if (string.IsNullOrWhiteSpace(nuevaPassword) || nuevaPassword.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres.");

            // 🔹 Crear DTO con los datos
            var dto = new ActualizarPasswordDto
            {
                IdUsuario = idUsuario,
                NuevaPassword = nuevaPassword
            };

            // 🔹 Enviar a la API
            return await _apiService.ActualizarPasswordAsync(dto, token, ct);
        }
        public async Task<(bool Ok, string Msg)> EnviarCorreoRecuperacionAsync(string correo, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return (false, "Debe ingresar un correo válido.");

            return await _apiService.EnviarCorreoRecuperacionAsync(correo, ct);
        }
        public async Task<(bool Ok, string Msg)> RestablecerContrasenaAsync(RestablecerContrasenaDto dto, CancellationToken ct = default)
        {
            // Validaciones del front (rápidas y claras)
            if (dto == null)
                return (false, "Solicitud inválida.");

            if (string.IsNullOrWhiteSpace(dto.Token))
                return (false, "El token es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.NuevaPassword))
                return (false, "La nueva contraseña es obligatoria.");

            // Reglas mínimas (ajusta si tienes regex global)
            if (dto.NuevaPassword.Length < 7)
                return (false, "La contraseña debe tener al menos 7 caracteres.");

            // Delegar al API service
            return await _apiService.RestablecerContrasenaAsync(dto, ct);
        }

    }
}
