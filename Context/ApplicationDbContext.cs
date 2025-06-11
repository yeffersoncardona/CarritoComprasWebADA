using Microsoft.EntityFrameworkCore;

namespace WebCarritoComprasAda.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Producto> Productos { get; set; }
        public DbSet<Models.CarritoItem> CarritoItems { get; set; }
    }
}
