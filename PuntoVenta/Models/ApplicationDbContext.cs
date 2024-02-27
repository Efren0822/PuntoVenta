using Microsoft.EntityFrameworkCore;

namespace PuntoVenta.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> UsuUsuario { get; set; }
    }

}
