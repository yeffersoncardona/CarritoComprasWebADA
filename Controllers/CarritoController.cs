using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCarritoComprasAda.Context;

namespace WebCarritoComprasAda.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int _usuarioId;
        public CarritoController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _usuarioId = Convert.ToInt32(_context.Usuarios.FirstOrDefault(x => x.Rol == "REGULAR"));
        }
        public IActionResult Index()
        {
            var Usuario = _context.Usuarios.Where( x => x.Rol =="REGULAR").FirstOrDefault();
            var carritoItems = _context.CarritoItems.Include(ci => ci.Producto).ToList();

            int totalItems = carritoItems.Sum(ci => ci.Cantidad);

            ViewBag.TotalItems = totalItems;
            ViewBag.UsuarioId = Usuario.Id;

            return View(carritoItems);
        }
        [HttpPost]
        public IActionResult AgregarAlCarritoAjax(int productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                return Json(new { success = false, message = "La cantidad debe ser mayor que cero." });
            }
            var producto = _context.Productos.Find(productoId);
            if (producto == null)
            {
                return Json( new { success=false, message= "Producto no encontrado." });
            }
            var carritoItem = _context.CarritoItems.FirstOrDefault(ci => ci.ProductoId == productoId);
            if (carritoItem == null)
            {
                carritoItem = new Models.CarritoItem
                {
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    UsuarioId = _usuarioId

                };
                _context.CarritoItems.Add(carritoItem);
            }
            else
            {
                carritoItem.Cantidad += cantidad;
                _context.CarritoItems.Update(carritoItem);
            }
            _context.SaveChanges();

            int totalItems = _context.CarritoItems.Sum(ci => ci.Cantidad);
            return Json(new { success = true, totalItems });
           
        }
        [HttpPost]
        public IActionResult EliminarDelCarritoAjax(int carritoItemId)
        {
            try
            {
                var carritoItem = _context.CarritoItems.Find(carritoItemId);
                if (carritoItem == null)
                {
                    return Json(new { success = false, message = "Carrito item no encontrado." });
                }
                _context.CarritoItems.Remove(carritoItem);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

    }
    }
