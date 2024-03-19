using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoVenta.Models
{
    public class Usuario
    {
      
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre de usuario es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El Nombre de usuario debe tener al menos 3 caracteres.")]
        public string strNombre { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "La Contraseña debe tener al menos 4 caracteres.")]
        public string strPassword { get; set; }

        [Required(ErrorMessage = "El campo Estado es requerido.")]
        public int IdUsuCatEstado { get; set; }

        [Required(ErrorMessage = "El campo Tipo Usuario es requerido.")]
        public int IdUsuCatTipoUsuario { get; set; }

        [ForeignKey("IdUsuCatEstado")]
        public UsuCatEstado Estado { get; set; }

        [ForeignKey("IdUsuCatTipoUsuario")]
        public UsuCatTipoUsuario TipoUsuario { get; set; }
    }
}
