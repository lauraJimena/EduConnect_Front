using System.Text.Json;

namespace EduConnect_Front.Utilities
{
    public static class SesionUtility
    {
        /// Guarda un objeto en la sesión convertido a JSON.
        public static void SetObject<TipoObjeto>(this ISession session, string key, TipoObjeto value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        /// Recupera un objeto de la sesión deserializándolo desde JSON.
        public static TipoObjeto? GetObject<TipoObjeto>(this ISession session, string key)
        {
            // Obtiene el string JSON guardado en sesión con la clave
            var json = session.GetString(key);

            // Si está vacío o nulo devuelve null
            if (string.IsNullOrWhiteSpace(json))
                return default;
            // Si existe contenido lo deserializa de JSON al objeto de tipo TipoObjeto
            return JsonSerializer.Deserialize<TipoObjeto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
