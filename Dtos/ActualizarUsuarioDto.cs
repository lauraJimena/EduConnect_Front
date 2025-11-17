using System.ComponentModel.DataAnnotations;

namespace EduConnect_Front.Dtos
{
    public class ActualizarUsuarioDto
    {
        [Required]
        public int IdUsu { get; set; }                // Identificador del usuario
        [Required]
        public string Nombre { get; set; } = string.Empty;   // Nombre del usuario
        [Required]
        public string Apellido { get; set; } = string.Empty; // Apellido del usuario
        [Required]
        public string Correo { get; set; } = string.Empty;   // Correo electrónico
        [Required]
        public int? IdTipoIdent { get; set; }                // Tipo de identificación (FK)
        [Required]
        public string NumIdent { get; set; } = string.Empty; // Número de documento
        [Required]
        public string TelUsu { get; set; } = string.Empty;   // Teléfono del usuario
        [Required]
        public string ContrasUsu { get; set; } = string.Empty; // Contraseña
        [Required]
        public int IdCarrera { get; set; }                  // Carrera (FK)
        [Required]
        public int? IdSemestre { get; set; }                 // Semestre (FK)
        [Required]
        public int? IdRol { get; set; }                      // Rol del usuario
        [Required]
        public int? IdEstado { get; set; }                   // Estado (Activo/Inactivo)
        [Required]

        public string Avatar{ get; set; } = string.Empty;


    }
}
