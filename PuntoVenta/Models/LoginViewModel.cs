using System.ComponentModel.DataAnnotations;
namespace PuntoVenta.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El Nombre de usuario debe tener al menos 3 caracteres.")]

        public string Username { get; set; }

        [Required(ErrorMessage ="La contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El Nombre de usuario debe tener al menos 3 caracteres.")]

        public string Password { get; set; }
    }
}
