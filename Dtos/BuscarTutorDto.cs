namespace EduConnect_Front.Dtos
{
    public class BuscarTutorDto
    {
        public string? Nombre { get; set; }
        public string? MateriaNombre { get; set; }
        public string? Semestre { get; set; }      // <- string
        public string? CarreraNombre { get; set; }
        public int? IdEstado { get; set; }         // 0 o null = no filtra
    }
}
