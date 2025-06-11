namespace WebCarritoComprasAda.Models
{
    using System.Linq;

    public class CarritoItem
    {
        public int carritoItemId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public Producto? Producto { get; set; }

    }
}
