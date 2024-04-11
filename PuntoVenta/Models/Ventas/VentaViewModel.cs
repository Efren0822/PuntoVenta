namespace PuntoVenta.Models.Ventas
{
    public class VentaViewModel
    {
        public List<ProductoVenta> Productos { get; set; }
        public string Folio { get; set; } // Agregar la propiedad Folio

        public int UserId { get; set; }

    }

    public class ProductoVenta
    {   public decimal Stock { get; set; }
        public int IdProProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

}
