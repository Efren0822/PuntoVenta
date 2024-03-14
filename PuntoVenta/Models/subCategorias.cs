namespace PuntoVenta.Models
{
    public class SubCategorias
    {
        public int IdSubCat { get; set; }
        public string strNombreSubCategoria { get; set; }
        public string strDescripcionSubCategoria { get; set; }
        public int idProCatCategoria { get; set; }
    }
}
