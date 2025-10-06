using EduConnect_Front.Dtos;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace EduConnect_Front.Services
{
    public class API_Service
    {
        private readonly HttpClient _httpClient;
        private const string baseUrl = "https://localhost:7003/";

        public API_Service()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }
        public async Task<(bool Success, string Message)> RegistrarUsuarioAsync(CrearUsuarioDto usuario, CancellationToken ct = default)
        {
            try
            {
                
                using var resp = await _httpClient.PostAsJsonAsync("General/RegistrarUsuario", usuario, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                    return (true, string.IsNullOrWhiteSpace(body) ? "Usuario registrado con éxito" : body);

                // Manejo genérico de error 
                var msg = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msg);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        public async Task<(bool Ok, string Msg, ObtenerUsuarioDto? Usuario)> IniciarSesionAsync(IniciarSesionDto dto, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("General/IniciarSesi%C3%B3n", dto, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var usuario = JsonSerializer.Deserialize<ObtenerUsuarioDto>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return (true, "Inicio de sesión correcto", usuario);
                }

                return (false, string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body, null);
            }
            catch (Exception ex)
            {
                return (false, $"Error al conectar con la API: {ex.Message}", null);
            }
        }
        // GET /Tutorado/{idTutorado}/historial
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Items)>
            ObtenerHistorialTutoradoAsync(int idTutorado, CancellationToken ct = default)
        {
            try
            {
                var url = $"Tutorado/{idTutorado}/historial";
                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<List<HistorialTutoriaDto>>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (true, "OK", data ?? new List<HistorialTutoriaDto>());
                }

                return (false, string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body, null);
            }
            catch (Exception ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
        }
        // GET /Administrador/ConsultarUsuarios
        public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Items)>
            ConsultarUsuariosAsync(CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.GetAsync("Administrador/ConsultarUsuarios", ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var items = JsonSerializer.Deserialize<List<ListadoUsuariosDto>>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new List<ListadoUsuariosDto>();

                    return (true, "OK", items);
                }

                var msg = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msg, null);
            }
            catch (Exception ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
        }
        // POST /Tutor/obtener
        public async Task<(bool Ok, string Msg, List<ObtenerTutorDto>? Items)>
            BuscarTutoresAsync(BuscarTutorDto filtros, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("Tutor/obtener", filtros, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var items = JsonSerializer.Deserialize<List<ObtenerTutorDto>>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new List<ObtenerTutorDto>();

                    return (true, "OK", items);
                }

                var msg = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msg, null);
            }
            catch (Exception ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
        }
        //Registro que hace el administrador
        public async Task<(bool Ok, string Msg)> RegistrarUsuarioAdminAsync(CrearUsuarioDto dto, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("Administrador/RegistrarUsuario", dto, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    //API devuelve "Usuario registrado con éxito" o texto similar
                    var msgOk = string.IsNullOrWhiteSpace(body) ? "Usuario registrado con éxito" : body;
                    return (true, msgOk);
                }

                var msgErr = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msgErr);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        // OBTENER USUARIO POR ID
        public async Task<(bool Ok, string Msg, ActualizarUsuarioDto? Usuario)> ObtenerUsuarioPorIdAsync(int idUsuario, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.GetAsync($"Administrador/ObtenerUsuarioPorId/{idUsuario}", ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var usuario = JsonSerializer.Deserialize<ActualizarUsuarioDto>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return (true, "Usuario obtenido correctamente", usuario);
                }

                var msgErr = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msgErr, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
        //ACTUALIZAR USUARIO

        public async Task<(bool Ok, string Msg)> ActualizarUsuarioAsync(ActualizarUsuarioDto dto, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.PutAsJsonAsync("Administrador/ActualizarUsuario", dto, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var msgOk = string.IsNullOrWhiteSpace(body) ? "Usuario actualizado con éxito" : body;
                    return (true, msgOk);
                }

                var msgErr = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msgErr);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        //INACTIVAR USUARIO
        public async Task<(bool Ok, string Msg)> EliminarUsuarioAsync(int idUsuario, CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.DeleteAsync($"Administrador/EliminarUsuario/{idUsuario}", ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    // Mensaje correcto si la API no devuelve cuerpo
                    var msgOk = string.IsNullOrWhiteSpace(body)
                        ? "Usuario eliminado (inactivado) correctamente."
                        : body;
                    return (true, msgOk);
                }

                // Si hubo error
                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;
                return (false, msgErr);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error al conectar con la API: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }



    }
}
