namespace EduConnect_Front.Dtos
{
    public class ActualizarUsuarioDto
    {
        public int IdUsu { get; set; }                // Identificador del usuario
        public string Nombre { get; set; } = string.Empty;   // Nombre del usuario
        public string Apellido { get; set; } = string.Empty; // Apellido del usuario
        public string Correo { get; set; } = string.Empty;   // Correo electrónico
        public int IdTipoIdent { get; set; }                // Tipo de identificación (FK)
        public string NumIdent { get; set; } = string.Empty; // Número de documento
        public string TelUsu { get; set; } = string.Empty;   // Teléfono del usuario
        public string ContrasUsu { get; set; } = string.Empty; // Contraseña
        public int IdCarrera { get; set; }                  // Carrera (FK)
        public int IdSemestre { get; set; }                 // Semestre (FK)
        public int IdRol { get; set; }                      // Rol del usuario
        public int IdEstado { get; set; }                   // Estado (Activo/Inactivo)
        public string Carrera { get; set; }= string.Empty;
        public string Rol { get; set; } = string.Empty; 
        public string Estado { get; set; } = string.Empty;  


    }
}
