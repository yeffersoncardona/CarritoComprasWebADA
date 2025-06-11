using System.ComponentModel.DataAnnotations;

namespace WebCarritoComprasAda.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombres { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        [Required, MaxLength(50)]
        public string NombreUsuario { get; set; }
        [Required, MaxLength(50)]
        public string Identificacion { get; set; }
        [Required]
        public string Contrasena { get; set; }
        [Required]
        public string Rol { get; set; } // 'Administrador' o 'Comprador'
    }
}
