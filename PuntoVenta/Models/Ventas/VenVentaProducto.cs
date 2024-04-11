using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Models.Ventas
{
    public class VenVentaProducto
    {
        [Key]
        public int idVenVentaProducto { get; set; }
        public int idVenVenta { get; set; }
        public int idProProducto { get; set; }
        public decimal decCantidad { get; set; }
        public decimal curTotal { get; set; }
    }
}
