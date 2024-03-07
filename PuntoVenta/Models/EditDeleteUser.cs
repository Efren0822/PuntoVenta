using System.ComponentModel.DataAnnotations;

namespace PuntoVenta.Models
{
    public class EditDeleteUser
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public string TipoUsuario { get; set; }
    }
}
