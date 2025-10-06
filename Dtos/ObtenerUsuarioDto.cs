namespace EduConnect_Front.Dtos
{
    public class ObtenerUsuarioDto
    {
        public int IdUsu { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public int IdEstado { get; set; }

        public string Rol { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public int IdSemestre { get; set; }
        public string NumIdent { get; set; } = string.Empty;
    }
}
