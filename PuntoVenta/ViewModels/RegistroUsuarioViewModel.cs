using System.Web.Mvc;
using PuntoVenta.Models;

namespace PuntoVenta.ViewModels
{
    public class RegistroUsuarioViewModel
    {
        public User User { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Estados { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> TiposUsuarios { get; set; }

        public string Mensaje { get; set; }
    }
}
