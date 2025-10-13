namespace EduConnect_Front.Dtos
{
    public class SolicitudTutoriaRespuestaDto
    {
        
        public int IdTutorado { get; set; }

    
        public int IdTutor { get; set; }

        public int IdMateria { get; set; }

       
        public int IdModalidad { get; set; }

        public DateTime? Fecha { get; set; }

        
        public string Hora { get; set; } = string.Empty;

        
        public string Tema { get; set; } = string.Empty;

     
        public string? ComentarioAdicional { get; set; }
    }
}
