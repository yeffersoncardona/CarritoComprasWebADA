using System;
using System.ComponentModel.DataAnnotations;

namespace WebCarritoComprasAda.Models
{
    public class Transacciones
    {
        [Key]
        public int TransaccionId { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int CantidadComprada { get; set; }
        public DateTime FechaCompra { get; set; }
        
    }
}
