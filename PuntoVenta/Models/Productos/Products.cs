using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Models.Productos
{
    public class Products
    {
        [Key]
        public int IdPro { get; set; }

        [Required(ErrorMessage = "este campo es requerido.")]
        [RegularExpression(@"^(?=(?:\S+\s){1,4}\S+$)(?![^\S\r\n]*\s{3,})(?!.*\b\s{2,}\b).*", ErrorMessage = "El nombre debe tener de una a cuatro palabras con máximo dos espacios entre cada palabra ej: Fanta de Cereza 1lits.")]
        public string StrNombrePro { get; set; }
        [Required(ErrorMessage = "este campo es requerido.")]
        public string StrDescriptcion { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression(@"^(?![01]\d).*\d+$", ErrorMessage = "El valor no puede ser negativo ni empezar con '0'.")]
        public int idProCatCategoria { get; set; } // Clave foránea para Categorias

        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression(@"^(?![01]\d).*\d+$", ErrorMessage = "El valor no puede ser negativo ni empezar con '0'.")]
        public int idProCatSubCategoria { get; set; } // Clave foránea para SubCategorias


        [Required(ErrorMessage = "este campo es requerido.")]

        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public decimal decMaximo { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public decimal decMinimo { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public decimal decStock { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public decimal curCosto { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public decimal curPrecio { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]

        public string strUrlImage { get; set; }

        public byte[]? blodImage { get; set; }




    }
}