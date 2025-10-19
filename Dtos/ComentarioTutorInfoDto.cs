namespace EduConnect_Front.Dtos
{
    public class ComentarioTutorInfoDto
    {
        public string Usuario { get; set; } = string.Empty; 
        public string Comentario { get; set; } = string.Empty;  
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
