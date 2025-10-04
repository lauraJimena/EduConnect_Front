namespace EduConnect_Front.Dtos
{
    public class ListadoUsuariosDto
    {
        public int IdUsu { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Correo { get; set; } = "";
        public int IdTipoIdent { get; set; }
        public string NumIdent { get; set; } = "";
        public string TelUsu { get; set; } = "";
        public string ContrasUsu { get; set; } = ""; // viene vacío en tu API
        public int IdCarrera { get; set; }
        public int IdSemestre { get; set; }
        public int IdRol { get; set; }
        public int IdEstado { get; set; }
    }
}
