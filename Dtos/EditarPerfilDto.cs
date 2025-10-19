namespace EduConnect_Front.Dtos
{
    public class EditarPerfilDto
    {
        public int IdUsu { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string TelUsu { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string ContrasUsu { get; set; } = string.Empty;
        public int IdTipoIdent { get; set; }
        public string NumIdent { get; set; } = string.Empty;
        public int? IdCarrera { get; set; }
        public int? IdSemestre { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarRadio{ get; set; }

    }
}
