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
        public decimal decMaximo { get; set; }
        public decimal decMinimo { get; set; }
        public decimal decStock { get; set; }
        public decimal curCosto { get; set; }
        public decimal curPrecio { get; set; }
      
        public string strUrlImage { get; set; }


        public virtual Categorias Categoria { get; set; } // Propiedad de navegación
        public virtual SubCategorias SubCategoria { get; set; } // Propiedad de navegación

    }
}