using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Models.Ventas
{
    public class VenVenta
    {

        [Key]
        public int idVenVenta { get; set; }
        public int idUsuUsuario { get; set; }
        public string strFolio { get; set; }
        public DateTime dtFecha { get; set; }
        public int idVenCatEstado { get; set; }
    }
}
