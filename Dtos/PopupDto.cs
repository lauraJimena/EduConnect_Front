namespace EduConnect_Front.Dtos
{
    public class PopupDto
    {
        public string? Tipo { get; set; }   // "success", "error", "info"
        public string? Titulo { get; set; }
        public string? Mensaje { get; set; }
    }
}
