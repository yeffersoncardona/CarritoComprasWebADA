using Microsoft.AspNetCore.Mvc;
using WebCarritoComprasAda.Context;
using WebCarritoComprasAda.Models;

namespace WebCarritoComprasAda.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductoController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();
            ViewBag.TotalProductos = _context.CarritoItems.Sum(ci => ci.Cantidad);

            return View(productos);
        }
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            return View(producto);
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
        [HttpPost]
        public IActionResult AgregarProducto([FromBody] Producto nuevoProducto)
        {
            try
            {
                if (nuevoProducto == null || string.IsNullOrEmpty(nuevoProducto.Nombre))
                {
                    return Json(new { success = false, message = "Datos inválidos." });
                }

                _context.Productos.Add(nuevoProducto);
                _context.SaveChanges();

                return Json(new { success = true, message = "Producto agregado exitosamente." });
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
