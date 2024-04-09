namespace PuntoVenta.Models.Ventas
{
    public class VenVentaPoducto
    {
        public int idVenVentaProducto { get; set; }
        public int idVenVenta { get; set; }
        public int idProproduto { get; set; }
        public decimal decCantidad { get; set; }
        public decimal curTotal { get; set; }
    }
}
