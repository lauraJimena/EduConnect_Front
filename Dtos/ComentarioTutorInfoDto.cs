namespace EduConnect_Front.Dtos
{
    public class ComentarioTutorInfoDto
    {
        public int IdComentario { get; set; }
        public string Usuario { get; set; } = string.Empty; 
        public string Comentario { get; set; } = string.Empty;  
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }

        public int IdEstado { get; set; } // 1 = activo, 2 = inactivo
    }
}
