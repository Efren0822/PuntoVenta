using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Models
{
    public class Products
    {
        public int IdPro { get; set; }
        public string StrNombrePro { get; set; }
        public string StrDescriptcion { get; set; }
        public int idProCatCategoria { get; set; } // Clave foránea para Categorias
        public int idProCatSubCategoria { get; set; }// Clave foránea para SubCategorias
        public double decMaximo { get; set; }
        public double decMinimo { get; set; }
        public int intStock { get; set; }
        public double curCosto { get; set; }
        public double curPrecio { get; set; }
      
        public string strUrlImage { get; set; }


        public virtual Categorias Categoria { get; set; } // Propiedad de navegación
        public virtual SubCategorias SubCategoria { get; set; } // Propiedad de navegación

    }
}