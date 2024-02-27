using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Models
{
    public class Usuario
    {
      
        public int Id { get; set; }
        public string strNombre { get; set; }
        public string strPassword { get; set; }
        public int IdUsuCatEstado { get; set; }
        public int IdUsuCatTipoUsuario { get; set; }

        [ForeignKey("IdUsuCatEstado")]
        public UsuCatEstado Estado { get; set; }

        [ForeignKey("IdUsuCatTipoUsuario")]
        public UsuCatTipoUsuario TipoUsuario { get; set; }
    }
}
