using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using WebCarritoComprasAda.Context;
using WebCarritoComprasAda.Models;

namespace WebCarritoComprasAda.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private int _usuarioId;
        public CarritoController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Usuario? usuario = new Usuario();
            usuario = _context.Usuarios.FirstOrDefault(x => x.Rol == "REGULAR");
            _usuarioId = usuario?.Id ?? 0; // Asignar el ID del usuario regular o 0 si no se encuentra
        }
        public IActionResult Index()
        {
            var Usuario = _context.Usuarios.Where(x => x.Rol == "REGULAR").FirstOrDefault();
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
                return Json(new { success = false, message = "Producto no encontrado." });
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
        [HttpPost]
        public async Task<IActionResult> RegistrarCompra()
        {
            Console.WriteLine("RegistrarCompra() ha sido invocado.");

            // Cambiar el tipo de carritoItems a List<CarritoItem> para que coincida con el método LINQ  
            List<CarritoItem> carritoItems = _context.CarritoItems
                .Include(ci => ci.Producto)
                .Where(ci => ci.UsuarioId == _usuarioId)
                .ToList();

            if (!carritoItems.Any())
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction("Index");
            }


            List<Transacciones> transacciones = new List<Transacciones>();

            foreach (var item in carritoItems)
            {
                transacciones.Add(new Transacciones
                {
                    UsuarioId = item.UsuarioId,
                    ProductoId = item.ProductoId,
                    CantidadComprada = item.Cantidad,
                    FechaCompra = DateTime.Now // Puedes ajustarlo si necesitas la fecha exacta de la compra
                });
            }
            // Aquí puedes completar la lógica para asignar valores a transacciones  
            // Ejemplo: transacciones.UsuarioId = _usuarioId;  

            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = "https://localhost:7093/api/transacciones/registrar";

            var jsonData = JsonSerializer.Serialize(transacciones);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "Compra realizada exitosamente.";
                //_context.CarritoItems.RemoveRange(carritoItems);  
                _context.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Error al procesar la compra.";
            }

            return RedirectToAction("Index");

        }
    }
}