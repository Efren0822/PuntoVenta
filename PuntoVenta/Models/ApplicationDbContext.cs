using Microsoft.EntityFrameworkCore;

namespace PuntoVenta.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> UsuUsuario { get; set; }
        public DbSet<UsuCatEstado> UsuCatEstado { get; set; }
        public DbSet<UsuCatTipoUsuario> UsuCatTipoUsuario { get; set; }

        public DbSet<User> Registros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("UsuUsuario");

        }

    }
    }