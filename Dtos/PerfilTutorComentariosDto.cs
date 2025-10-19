namespace EduConnect_Front.Dtos
{
    public class PerfilTutorComentariosDto
    {
        // Información principal del tutor
        public PerfilTutorDto Perfil { get; set; }= new();

        // Lista de comentarios asociados a ese tutor
        public List<ComentarioTutorInfoDto> Comentarios { get; set; } = new();
    }
}
