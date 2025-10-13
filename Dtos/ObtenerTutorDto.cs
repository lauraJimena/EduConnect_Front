namespace EduConnect_Front.Dtos
{
    public class ObtenerTutorDto
    {
        public int IdUsuario { get; set; }
        public string TutorNombreCompleto { get; set; } = "";
        public int IdEstado { get; set; }
        public int IdMateria { get; set; }
        public string? MateriaNombre { get; set; }
        public int Semestre { get; set; }
        public string? CarreraNombre { get; set; }
    }
}
