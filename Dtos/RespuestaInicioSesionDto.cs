namespace EduConnect_Front.Dtos
{
    public class RespuestaInicioSesionDto
    {
        public int IdUsuario { get; set; }
        public string? Token { get; set; }
        public DateTime TiempoExpiracion { get; set; }
        public int Respuesta { get; set; }
        public string? Mensaje { get; set; }
        public string? AvatarUrl { get; set; }
        public bool DebeActualizarPassword { get; set; }
    }
}
