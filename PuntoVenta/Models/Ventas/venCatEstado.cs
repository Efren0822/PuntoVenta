using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Models.Ventas
{
    public class venCatEstado
    {
        [Key]
        public int idVenCatEstado { get; set; }
        public string strNombre { get; set; }
        public string strDescripcion { get; set; }

    }
}
