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

        public DbSet<Products> Productos { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<SubCategorias> SubCategorias { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("UsuUsuario");
            modelBuilder.Entity<Products>().ToTable("ProProducto")
                .HasKey(b => b.IdPro);
            modelBuilder.Entity<Products>()
        .HasOne(p => p.Categoria)
        .WithMany()
        .HasForeignKey(p => p.idProCatCategoria); // Asegúrate de que la propiedad se llama 'IdCat' y no 'CategoriaIdCat'

            modelBuilder.Entity<Products>()
                .HasOne(p => p.SubCategoria)
                .WithMany()
                .HasForeignKey(p => p.idProCatSubCategoria);

            modelBuilder.Entity<Categorias>().ToTable("ProCatCategoria")
                .HasKey(c => c.IdCat);
            modelBuilder.Entity<SubCategorias>().ToTable("ProCatSubCategoria")
                .HasKey(sc => sc.IdSubCat);

        }

    }
}