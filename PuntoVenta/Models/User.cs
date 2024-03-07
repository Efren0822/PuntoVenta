using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Models
{
    public class User
    {
        public int Id { get; set; }

        
        public string Username { get; set; }

       
        public string Password { get; set; }

        [Required]
        public int IdUsuCatEstado { get; set; }
        public UsuCatEstado usuCatEstado { get; set; }

        [Required]
        public int IdUsuCatTipoUsuario { get; set; }
        public UsuCatTipoUsuario tipoUsuarios { get; set; }

    }
}
