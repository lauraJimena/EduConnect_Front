namespace EduConnect_Front.Dtos
{
    public class CrearMensajeDto
    {
        public int IdChat { get; set; }
        public int IdEmisor { get; set; }
        public string Contenido { get; set; } = string.Empty;
    }
}
