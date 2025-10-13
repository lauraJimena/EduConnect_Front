namespace EduConnect_Front.Dtos
{
    public class HistorialTutoriaDto
    {
        
        public int IdTutoria { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        public string? Tema { get; set; }
        public string? ComentarioAdic { get; set; }

        public byte IdModalidad { get; set; }
        public int IdTutorado { get; set; }
        public int IdTutor { get; set; }
        public int IdMateria { get; set; }
        public byte IdEstado { get; set; }

       
        public string? ModalidadNombre { get; set; }
        public string? MateriaNombre { get; set; }
        public string? EstadoNombre { get; set; }

       
        public string? TutorNombreCompleto { get; set; }

    }
}
