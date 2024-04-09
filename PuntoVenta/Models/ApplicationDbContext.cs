using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models.Productos;
using PuntoVenta.Models.Ventas;

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

        public DbSet<Products> Productos { get; set; }

        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<SubCategorias> SubCategorias { get; set; }
       // public DbSet<VenVentaPoducto> venVentaProductos { get; set; }  


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("UsuUsuario");

         
            //products se utiliza para la visualizacion de los productos
            modelBuilder.Entity<Products>().ToTable("ProProducto")
                   
                              .HasKey(p => p.IdPro);



          

       
            // Configuración de la precisión para propiedades decimales
            modelBuilder.Entity<Products>().Property(p => p.decMaximo).HasPrecision(18, 2);
            modelBuilder.Entity<Products>().Property(p => p.decMinimo).HasPrecision(18, 2);
            modelBuilder.Entity<Products>().Property(p => p.decStock).HasPrecision(18, 2);
            modelBuilder.Entity<Products>().Property(p => p.curCosto).HasPrecision(18, 2);
            modelBuilder.Entity<Products>().Property(p => p.curPrecio).HasPrecision(18, 2);


            modelBuilder.Entity<Categorias>().ToTable("ProCatCategoria")
                .HasKey(c => c.IdCat);
            modelBuilder.Entity<SubCategorias>().ToTable("ProCatSubCategoria")
                .HasKey(sc => sc.IdSubCat);

        }

    }
}