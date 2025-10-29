using EduConnect_Front.Dtos;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        public async Task<(bool Success, string Message)> RegistrarUsuarioAsync(CrearUsuarioDto usuario, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                   new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
        public async Task<(bool Ok, string Msg, List<SolicitudTutorDto>? Solicitudes)>
    ObtenerSolicitudesTutoriasAsync(FiltroSolicitudesTutorDto filtro, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var resp = await _httpClient.PostAsJsonAsync("Tutor/SolicitudesTutorias", filtro, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var lista = JsonSerializer.Deserialize<List<SolicitudTutorDto>>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (true, "Solicitudes obtenidas correctamente", lista);
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



        public async Task<(bool Ok, string Msg, RespuestaInicioSesionDto? Usuario)> IniciarSesionAsync(
      IniciarSesionDto dto,
      CancellationToken ct = default)
        {
            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("General/IniciarSesión", dto, ct);

                var body = await resp.Content.ReadAsStringAsync(ct);

                // 🔹 Si el cuerpo está vacío
                if (string.IsNullOrWhiteSpace(body))
                    return (false, $"Error {(int)resp.StatusCode}: Respuesta vacía del servidor.", null);

                // 🔹 Intentar parsear JSON solo si parece JSON
                RespuestaInicioSesionDto? usuario = null;
                if (body.TrimStart().StartsWith("{") || body.TrimStart().StartsWith("["))
                {
                    try
                    {
                        usuario = JsonSerializer.Deserialize<RespuestaInicioSesionDto>(
                            body,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch
                    {
                        // Si el contenido no es JSON válido, lo tratamos como texto
                    }
                }

                // ✅ Si la respuesta fue exitosa (HTTP 200) y se pudo deserializar
                if (resp.IsSuccessStatusCode && usuario != null)
                {
                    return (true, usuario.Mensaje ?? "Inicio de sesión exitoso", usuario);
                }

                // 🔹 Si hubo error (texto plano o mensaje en JSON)
                string msgError;
                if (usuario != null && !string.IsNullOrWhiteSpace(usuario.Mensaje))
                {
                    msgError = usuario.Mensaje!;
                }
                else
                {
                    msgError = body.Trim('"', '\n', '\r'); // limpiar comillas o saltos
                }

                return (false, msgError, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error al conectar con la API: {ex.Message}", null);
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud de inicio de sesión expiró (timeout).", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
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
        // CONSULTAR USUARIOS ADMIN 
        public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Usuarios)> ObtenerUsuariosAsync(
            string token,
            int? idRol = null,
            int? idEstado = null,
            string? numIdent = null,
            CancellationToken ct = default)
        {
            try
            {
                // 🔹 Agregar encabezado de autorización con el token JWT
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // 🔹 Construimos la URL con los filtros dinámicos
                var queryParams = new List<string>();
                if (idRol.HasValue) queryParams.Add($"idRol={idRol.Value}");
                if (idEstado.HasValue) queryParams.Add($"idEstado={idEstado.Value}");
                if (!string.IsNullOrWhiteSpace(numIdent)) queryParams.Add($"numIdent={numIdent}");

                var url = "Administrador/ConsultarUsuarios";
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                // 🔹 Hacemos la petición
                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                // ✅ Éxito
                if (resp.IsSuccessStatusCode)
                {
                    var usuarios = System.Text.Json.JsonSerializer.Deserialize<List<ListadoUsuariosDto>>(
                        body,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return (true, "Usuarios obtenidos correctamente", usuarios);
                }

                // ❌ Error controlado
                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;
                return (false, msgErr, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
        public async Task<List<ObtenerTutorDto>> BuscarTutoresAsync(BuscarTutorDto filtros, string token)
        {
            try
            {
                // Configurar el token JWT
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Hacer la petición POST
                var response = await _httpClient.PostAsJsonAsync("Tutorado/BuscarTutor", filtros);

                if (response.IsSuccessStatusCode)
                {
                    var tutores = await response.Content.ReadFromJsonAsync<List<ObtenerTutorDto>>();
                    return tutores ?? new List<ObtenerTutorDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error del servidor: {error}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"No se pudo conectar con el servidor: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al obtener tutores: {ex.Message}");
            }
        }

        //REGISTRAR USUARIO ADMIN
        public async Task<(bool Ok, string Msg)> RegistrarUsuarioAdminAsync(CrearUsuarioDto dto, string token, CancellationToken ct = default)
        {
            try
            {
                // Configurar el token JWT
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
        public async Task<(bool Ok, string? Msg, ActualizarUsuarioDto? Usuario)> ObtenerUsuarioPorIdPerfil(int id, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"Administrador/ObtenerUsuarioPorId/{id}");
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var usuario = JsonSerializer.Deserialize<ActualizarUsuarioDto>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    
                    return (true, null, usuario);
                }

                return (false, body, null);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener usuario: {ex.Message}", null);
            }
        }

        public async Task<(bool Ok, string Msg, ObtenerUsuarioDto? Usuario)> ObtenerUsuarioPorIdAsync(
    int idUsuario,
    string token,
    CancellationToken ct = default)
        {
            try
            {
                // Agregar token en el encabezado de la solicitud
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var resp = await _httpClient.GetAsync($"Administrador/ObtenerUsuarioPorId/{idUsuario}", ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var usuario = JsonSerializer.Deserialize<ObtenerUsuarioDto>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (true, "Usuario obtenido correctamente", usuario);
                }

                return (false, $"Error {(int)resp.StatusCode}: {body}", null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error de conexión: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }

        // ACTUALIZAR USUARIO
        public async Task<(bool Ok, string Msg)> ActualizarUsuarioAsync(
            ActualizarUsuarioDto dto,
            string token,
            CancellationToken ct = default)
        {
            try
            {
                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

              
                using var resp = await _httpClient.PutAsJsonAsync("Administrador/ActualizarUsuario", dto, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

               
                if (resp.IsSuccessStatusCode)
                {
                    var msgOk = string.IsNullOrWhiteSpace(body)
                        ? "Usuario actualizado con éxito"
                        : body;

                    return (true, msgOk);
                }

                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;

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
        public async Task<string> ActualizarPerfilAsync(EditarPerfilDto perfil, string token)
        {
            // Agregar encabezado de autorización Bearer
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Hacer la llamada PUT
            var response = await _httpClient.PutAsJsonAsync("Tutorado/ActualizarPerfil", perfil);

            // Manejo de respuesta
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // devuelve "Perfil actualizado con éxito"
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar perfil: {error}");
            }
        }
        public async Task<string> ActualizarPerfilTutor(EditarPerfilDto perfil, string token)
        {
            // Agregar encabezado de autorización Bearer
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Hacer la llamada PUT
            var response = await _httpClient.PutAsJsonAsync("Tutor/ActualizarPerfil", perfil);

            // Manejo de respuesta
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // devuelve "Perfil actualizado con éxito"
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar perfil: {error}");
            }
        }

        public async Task<(bool Ok, string Msg)> EditarPerfilAsync(
    EditarPerfilDto dto,

    CancellationToken ct = default)
        {
            try
            {


                // Enviar la solicitud PUT al endpoint
                using var resp = await _httpClient.PutAsJsonAsync("Tutorado/ActualizarPerfil", dto, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    // Mensaje exitoso, el backend devuelve texto plano ("Perfil actualizado con éxito")
                    var msgOk = string.IsNullOrWhiteSpace(body)
                        ? "Perfil actualizado correctamente."
                        : body;
                    return (true, msgOk);
                }

                // 🔸 Si hubo error en la respuesta
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

        //INACTIVAR USUARIO
        public async Task<(bool Ok, string Msg)> EliminarUsuarioAsync(int idUsuario, string token, CancellationToken ct = default)
        {
            try
            {
                // Validar token antes de usarlo
                if (string.IsNullOrWhiteSpace(token))
                    return (false, "Token de autenticación inválido o no encontrado.");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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


        // OBTENER HISTORIAL DE TUTORÍAS PARA EL TUTORADO
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Datos)> ObtenerHistorialTutoradoAsync(
            int idTutorado,
            string token,                        
            List<int>? idsEstado = null,
            CancellationToken ct = default)
        {
            try
            {
                // Validar token antes de usarlo
                if (string.IsNullOrWhiteSpace(token))
                    return (false, "Token de autenticación inválido o no encontrado.", null);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                
                var url = $"Tutorado/{idTutorado}/historial";

                if (idsEstado != null && idsEstado.Any())
                {
                    var query = string.Join("&", idsEstado.Select(e => $"idsEstado={e}"));
                    url += $"?{query}";
                }

                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                
                if (resp.IsSuccessStatusCode)
                {
                    var datos = JsonSerializer.Deserialize<List<HistorialTutoriaDto>>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return (true, "Historial obtenido correctamente", datos ?? new());
                }

                
                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}"
                    : body;

                return (false, msgErr, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error al conectar con la API: {ex.Message}", null);
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud de historial expiró (timeout).", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }

        //OBTENER CARRERAS
        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            var response = await _httpClient.GetAsync("General/ObtenerCarreras");

            if (response.IsSuccessStatusCode)
            {
                var carreras = await response.Content.ReadFromJsonAsync<List<CarreraDto>>();
                return carreras ?? new List<CarreraDto>();
            }

            throw new Exception("Error al obtener las carreras del API");
        }
        public async Task<List<TipoIdentDto>> ObtenerTipoIdentAsync()
        {
            var response = await _httpClient.GetAsync("General/ObtenerTiposIdent");

            if (response.IsSuccessStatusCode)
            {
                var tipoIdent = await response.Content.ReadFromJsonAsync<List<TipoIdentDto>>();
                return tipoIdent ?? new List<TipoIdentDto>();
            }

            throw new Exception("Error al obtener las carreras del API");
        }

        //OBETENER USUARIO POR ID
        public async Task<ObtenerUsuarioDto?> ObtenerUsuarioPorIdAsync(int idUsuario, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Administrador/ObtenerUsuarioPorId/{idUsuario}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener usuario: {body}");

           
            var opts = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return System.Text.Json.JsonSerializer.Deserialize<ObtenerUsuarioDto>(body, opts);
        }

        //SOLICITUDES TUTORIAS TUTORADO
        public async Task<List<SolicitudTutoriaDto>> ObtenerSolicitudesTutoriasAsync(
    FiltroSolicitudesDto filtro, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var resp = await _httpClient.PostAsJsonAsync("Tutorado/SolicitudesTutorias", filtro, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var solicitudes = JsonSerializer.Deserialize<List<SolicitudTutoriaDto>>(
                        body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return solicitudes ?? new List<SolicitudTutoriaDto>();
                }

                // Si hubo error
                var msgError = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;
                throw new Exception(msgError);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con la API: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                throw new Exception("La solicitud de tutorías tardó demasiado (timeout).");
            }
        }

        //CREAR SOLICITUD TUTORIA
        public async Task<(bool Success, string Message, int IdTutoria)> CrearSolicitudTutoriaAsync(
    SolicitudTutoriaRespuestaDto solicitud,
    string token,
    CancellationToken ct = default)
        {
            try
            {
                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var resp = await _httpClient.PostAsJsonAsync("Tutorado/CrearSolicitudTutoria", solicitud, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var opciones = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var resultado = JsonSerializer.Deserialize<Dictionary<string, object>>(body, opciones);

                    if (resultado != null && resultado.ContainsKey("idTutoria"))
                    {
                        int idTutoria = int.Parse(resultado["idTutoria"].ToString());
                        string mensaje = resultado.ContainsKey("mensaje") ? resultado["mensaje"].ToString() : "Solicitud creada correctamente";

                        return (true, mensaje, idTutoria);
                    }

                    return (true, "Solicitud creada correctamente", 0);
                }


                //Manejo de errores devueltos por la API (400, 401, 500, etc.)
                var msg = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;

                if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    msg = "Tu sesión ha expirado o el token no es válido. Por favor, inicia sesión nuevamente.";

                return (false, msg, 0);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"❌ No se pudo conectar con la API: {ex.Message}", 0);
            }
            catch (TaskCanceledException)
            {
                return (false, "⏰ La solicitud a la API expiró (timeout).", 0);
            }
            catch (Exception ex)
            {
                return (false, $"⚠️ Error inesperado: {ex.Message}", 0);


            }

        }
        //ACEPTAR SOLICITUD DE TUTORÍA
        public async Task<(bool Ok, string Msg)> AceptarSolicitudTutoriaAsync(int idTutoria, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new ActualizarEstadoSolicitudDto { IdTutoria = idTutoria };

                using var resp = await _httpClient.PutAsJsonAsync("Tutor/AceptarSolicitudTutoria", request, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                    return (true, string.IsNullOrWhiteSpace(body) ? "Solicitud aceptada correctamente" : body);

                var msgErr = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body;
                return (false, msgErr);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        //RECHAZAR SOLICITUD DE TUTORÍA
        public async Task<(bool Ok, string Msg)> RechazarSolicitudTutoriaAsync(int idTutoria, string token, CancellationToken ct = default)
        {
            try
            {
                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new ActualizarEstadoSolicitudDto { IdTutoria = idTutoria };

                
                using var resp = await _httpClient.PutAsJsonAsync("Tutor/RechazarSolicitudTutoria", request, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                    return (true, string.IsNullOrWhiteSpace(body) ? "Solicitud rechazada correctamente" : body);

                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;

                return (false, msgErr);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        //HISTORIAL DE TUTORÍAS PARA EL TUTOR
        public async Task<(bool Ok, string Msg, List<HistorialTutoriaDto>? Data)> ObtenerHistorialTutorAsync(
    int idTutor,
    List<int>? estados,
    string token,
    CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

               
                string url = $"Tutor/{idTutor}/historial/";
                if (estados != null && estados.Any())
                {
                    string query = string.Join("&", estados.Select(e => $"idEstados={e}"));
                    url += $"?{query}";
                }

                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                {
                    return (false, $"Error al obtener historial: {body}", null);
                }

                var datos = JsonSerializer.Deserialize<List<HistorialTutoriaDto>>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return (true, "Historial obtenido correctamente", datos ?? new List<HistorialTutoriaDto>());
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
        // ACTUALIZAR PERFIL DE TUTOR
        public async Task<(bool Ok, string Msg)> ActualizarPerfilTutorAsync(EditarPerfilDto perfil, string token, CancellationToken ct = default)
        {
            try
            {
                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.PutAsJsonAsync("Tutor/ActualizarPerfil", perfil, ct);
                var body = await response.Content.ReadAsStringAsync(ct);

                if (response.IsSuccessStatusCode)
                    return (true, string.IsNullOrWhiteSpace(body) ? "Perfil actualizado con éxito" : body);

                var msgError = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)response.StatusCode}"
                    : body;

                return (false, msgError);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }
        // CONSULTAR USUARIOS ADMIN 
        public async Task<(bool Ok, string Msg, List<ListadoUsuariosDto>? Usuarios)> ObtenerUsuariosAdmin(
            string token,
            int? idRol = null,
            int? idEstado = null,
            string? numIdent = null,
            CancellationToken ct = default)
        {
            try
            {
               
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                
                var queryParams = new List<string>();
                if (idRol.HasValue) queryParams.Add($"idRol={idRol.Value}");
                if (idEstado.HasValue) queryParams.Add($"idEstado={idEstado.Value}");
                if (!string.IsNullOrWhiteSpace(numIdent)) queryParams.Add($"numIdent={numIdent}");

                var url = "Administrador/ConsultarUsuarios";
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

              
                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

            
                if (resp.IsSuccessStatusCode)
                {
                    var usuarios = System.Text.Json.JsonSerializer.Deserialize<List<ListadoUsuariosDto>>(
                        body,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return (true, "Usuarios obtenidos correctamente", usuarios);
                }

               
                var msgErr = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;
                return (false, msgErr, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", null);
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
        //RANKING DE TUTORES
        public async Task<(bool Ok, string Msg, List<RankingTutorDto> Data)> ObtenerRankingTutoresAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("Tutorado/RankingTutores");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return (false, $"Error {response.StatusCode}: {errorBody}", null);
                }

                var data = await response.Content.ReadFromJsonAsync<List<RankingTutorDto>>();
                return (true, "Ranking cargado correctamente", data ?? new List<RankingTutorDto>());
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error de conexión con la API: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
        //PERFIL TUTOR
        public async Task<(bool Ok, string Msg, PerfilTutorDto? Data)> ObtenerPerfilTutorAsync(int idTutor, string token)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"Tutorado/PerfilTutor/{idTutor}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true 
                    };

                    var perfil = System.Text.Json.JsonSerializer.Deserialize<PerfilTutorDto>(body, options);
                    return (true, "Perfil obtenido correctamente", perfil);
                }

                return (false, $"Error {(int)response.StatusCode}: {body}", null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error de conexión con la API: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
        // OBTENER TUTORADO POR ID
        public async Task<ObtenerUsuarioDto?> ObtenerTutoradoPorIdAsync(int idUsuario, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Tutorado/ObtenerTutoradoPorId/{idUsuario}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener tutorado: {body}");

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ObtenerUsuarioDto>(body, opts);
        }
        //// OBTENER TUTOR POR ID
        //public async Task<ObtenerUsuarioDto?> ObtenerTutorPorIdAsync(int idUsuario, string token)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"Tutor/ObtenerTutorPorId/{idUsuario}");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    using var response = await _httpClient.SendAsync(request);
        //    var body = await response.Content.ReadAsStringAsync();

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //        return null;

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception($"Error al obtener tutorado: {body}");

        //    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    return JsonSerializer.Deserialize<ObtenerUsuarioDto>(body, opts);
        //}
        // ACTUALIZAR PERFIL TUTORADO
        public async Task<string> ActualizarPerfilTutorado(EditarPerfilDto perfil, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync("Tutorado/ActualizarPerfil", perfil);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al actualizar perfil: {error}");
        }
        // OBTENER TUTORADO POR ID
        public async Task<ObtenerUsuarioDto?> ObtenerTutorPorIdAsync(int idUsuario, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Tutor/ObtenerTutorPorId/{idUsuario}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener tutorado: {body}");

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ObtenerUsuarioDto>(body, opts);
        }
        // OBTENER COMENTARIOS POR TUTOR
        public async Task<IEnumerable<ComentarioTutorInfoDto>> ObtenerComentariosPorTutorAsync(int idTutor, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new { idTutor = idTutor };

            var response = await _httpClient.PostAsJsonAsync("Tutorado/ComentariosTutor", request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<ComentarioTutorInfoDto>>();

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al obtener comentarios: {error}");
        }
        //CREAR COMENTARIO 
        public async Task<string> CrearComentarioAsync(CrearComentarioDto comentario, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("Tutorado/CrearComentario", comentario);

            if (response.IsSuccessStatusCode)
            {
                // El backend devuelve { mensaje = "Comentario creado correctamente." }
                var resultado = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return resultado?["mensaje"] ?? "Comentario creado correctamente.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear el comentario: {error}");
            }
        }
        public async Task<(bool Ok, bool TieneMaterias, string Msg)> ValidarMateriasTutorAsync(int idTutor, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"Tutor/ValidarMaterias/{idTutor}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (false, false, $"Error al validar materias: {body}");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body, options);

                bool tieneMaterias = json["tieneMaterias"].GetBoolean();
                string mensaje = json["mensaje"].GetString() ?? "";

                return (true, tieneMaterias, mensaje);
            }
            catch (Exception ex)
            {
                return (false, false, "Error de conexión: " + ex.Message);
            }
        }
        public async Task<(bool Ok, string Msg, List<MateriaDto>? Data)> ObtenerMateriasPorTutorAsync(int idTutor, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"Tutor/MateriasPorTutor/{idTutor}");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (false, $"Error {(int)response.StatusCode}: {body}", null);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<List<MateriaDto>>(body, options);

                return (true, "Materias obtenidas correctamente", data);
            }
            catch (Exception ex)
            {
                return (false, "Error de conexión con la API: " + ex.Message, null);
            }
        }

        public async Task<(bool Ok, string Msg)> RegistrarMateriasTutorAsync(int idTutor, int[] materias, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var body = new { IdTutor = idTutor, Materias = materias };
                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Tutor/RegistrarMaterias", content);
                var bodyResp = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, "Materias registradas correctamente");

                return (false, $"Error {(int)response.StatusCode}: {bodyResp}");
            }
            catch (Exception ex)
            {
                return (false, "Error al conectar con la API: " + ex.Message);
            }
        }


        public async Task<List<ObtenerChatDto>> ObtenerChatsAsync(int idUsuario, string token, CancellationToken ct = default)
        {

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var resp = await _httpClient.GetAsync($"Chats/ObtenerChatsPorUsuario?usuarioId={idUsuario}", ct);
                var body = await resp.Content.ReadAsStringAsync(ct);


                if (resp.IsSuccessStatusCode)
                {

                    var chats = JsonSerializer.Deserialize<List<ObtenerChatDto>>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return chats ?? new List<ObtenerChatDto>();
                }

                var msgError = string.IsNullOrWhiteSpace(body)
                    ? $"Error HTTP {(int)resp.StatusCode}"
                    : body;


                throw new Exception(msgError);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" 🔴 EXCEPCIÓN en ObtenerChatsAsync -> " + ex.Message);
                throw; // deja que el controlador lo capture
            }
        }


        // ---------------------------
        // Obtener Mensajes por chat
        // ---------------------------
        public async Task<List<ObtenerMensajeDto>> ObtenerMensajesAsync(int idChat, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                using var resp = await _httpClient.GetAsync($"Chats/ObtenerMensajes?chatId={idChat}", ct);
                var body = await resp.Content.ReadAsStringAsync(ct); 
                if (resp.IsSuccessStatusCode)
                { 
                    var mensajes = JsonSerializer.Deserialize<List<ObtenerMensajeDto>>(body, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true }); return mensajes ?? new List<ObtenerMensajeDto>();
                } 
                var msgError = string.IsNullOrWhiteSpace(body) ? $"Error {(int)resp.StatusCode}" : body; throw new Exception(msgError); 
            } catch (HttpRequestException ex) 
            { 
                throw new Exception("ObtenerMensajesAsync -> " + ex.ToString());
            } 
            catch (TaskCanceledException) 
            { 
                throw new Exception("La solicitud de mensajes tardó demasiado (timeout).");
            } 
        }

        // ---------------------------
        // Enviar / Crear Mensaje
        // ---------------------------
        public async Task<(bool Success, string Message)> EnviarMensajeAsync(CrearMensajeDto mensaje, string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("[API_Service] POST Chats/CrearMensaje -> " +
                                  $"Chat:{mensaje.IdChat} Emisor:{mensaje.IdEmisor}");

                using var resp = await _httpClient.PostAsJsonAsync("Chats/CrearMensaje", mensaje, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (resp.IsSuccessStatusCode)
                {
                    var okMsg = string.IsNullOrWhiteSpace(body) ? "Mensaje enviado correctamente" : body;
                    return (true, okMsg);
                }

                var msg = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)resp.StatusCode}"
                    : body;

                return (false, msg);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API (EnviarMensajeAsync): {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud a la API expiró (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado en EnviarMensajeAsync: {ex.Message}");
            }
        }
        public async Task<(bool Ok, string Msg, List<ReporteTutorDto> Reporte)> ObtenerReporteTutoresAsync(string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.GetAsync("Administrador/ReporteTutores", ct);
                var body = await response.Content.ReadAsStringAsync(ct);

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<List<ReporteTutorDto>>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return (true, "Reporte obtenido correctamente", data ?? new List<ReporteTutorDto>());
                }

                var msgError = string.IsNullOrWhiteSpace(body)
                    ? $"Error {(int)response.StatusCode}"
                    : body;

                return (false, msgError, new List<ReporteTutorDto>());
            }
            catch (HttpRequestException ex)
            {
                return (false, $"No se pudo conectar con la API: {ex.Message}", new List<ReporteTutorDto>());
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", new List<ReporteTutorDto>());
            }
        }
        public async Task<List<ReporteTutoradoDto>> ObtenerReporteTutoradosActivosAsync(string token, CancellationToken ct = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.GetAsync("Administrador/ReporteTutoradosActivos", ct);
                var body = await response.Content.ReadAsStringAsync(ct);

                if (response.IsSuccessStatusCode)
                {
                    var lista = JsonSerializer.Deserialize<List<ReporteTutoradoDto>>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return lista ?? new List<ReporteTutoradoDto>();
                }

                throw new Exception($"Error {response.StatusCode}: {body}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el reporte de tutorados activos: " + ex.Message);
            }
        }
        public async Task<bool> EnviarConfirmacionTutoriaAsync(string token, int idTutoria)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"Tutorado/EnviarConfirmacionTutoria?idTutoria={idTutoria}", null);

            if (response.IsSuccessStatusCode)
                return true;

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al enviar el correo: {error}");
        }

        public async Task<List<TutoriaConsultaDto>> ConsultarTutoriasCoordAsync(
             string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha, string token, CancellationToken ct = default)
        {
            // 👇 Importante: autorización
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // 👇 Asegura que coincide con la ruta exacta del endpoint
            var url = "Coordinador/ConsultarTutorias?";

            // ✅ Agrega filtros solo si existen
            if (!string.IsNullOrWhiteSpace(carrera)) url += $"carrera={Uri.EscapeDataString(carrera)}&";
            if (semestre.HasValue) url += $"semestre={semestre}&";
            if (!string.IsNullOrWhiteSpace(materia)) url += $"materia={Uri.EscapeDataString(materia)}&";
            if (idEstado.HasValue) url += $"idEstado={idEstado}&";
            if (ordenFecha.HasValue) url += $"ordenFecha={ordenFecha}&";

            try
            {
                using var resp = await _httpClient.GetAsync(url, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Error al consultar tutorías: {body}");

                var tutorias = JsonSerializer.Deserialize<List<TutoriaConsultaDto>>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return tutorias ?? new List<TutoriaConsultaDto>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener tutorías desde la API: " + ex.Message);
            }
        }
        public async Task<ReporteDemandaAcademicaDto?> ObtenerReporteDemandaAcademicaAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.GetAsync("Coordinador/ReporteDemandaAcademica");
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReporteDemandaAcademicaDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        public async Task<(ReporteGestionAdministrativaDto? Totales, List<ReporteDesempenoTutorDto>? Desempeno)>
            ObtenerReporteCombinadoAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var resp = await _httpClient.GetAsync("Coordinador/ReporteCombinado");

                if (!resp.IsSuccessStatusCode)
                    return (null, null);

                var json = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var totales = JsonSerializer.Deserialize<ReporteGestionAdministrativaDto>(
                    root.GetProperty("totales").GetRawText(), options);
                var desempeno = JsonSerializer.Deserialize<List<ReporteDesempenoTutorDto>>(
                    root.GetProperty("desempeno").GetRawText(), options);

                return (totales, desempeno);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }
        public async Task<List<ComentarioTutorDto>?> ObtenerComentariosTutorAsync(FiltrosComentariosTutorDto filtro, string token)
        {
            try
            {
                // 🔹 Autenticación con el token JWT
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // 🔹 Envía el filtro como JSON con POST
                var response = await _httpClient.PostAsJsonAsync("Tutor/Comentarios", filtro);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error HTTP {response.StatusCode} al obtener comentarios del tutor");
                    return null;
                }

                // 🔹 Leer el cuerpo JSON de la respuesta
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 🔹 Deserializar la lista de comentarios
                var comentarios = JsonSerializer.Deserialize<List<ComentarioTutorDto>>(json, options);

                return comentarios ?? new List<ComentarioTutorDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error en ObtenerComentariosTutorAsync: {ex.Message}");
                return null;
            }
        }








    }
}