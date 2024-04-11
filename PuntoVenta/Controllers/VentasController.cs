
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using PuntoVenta.Models.Ventas;

namespace PuntoVenta.Controllers
{
    public class VentasController : Controller
    {
        public ActionResult MostrarUsuario()
        {


            Console.WriteLine("Controlador de ventas");
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                Console.WriteLine($"Usuario obtenido correctamente: {username}");
                return View();
            }
            else
            {
                Console.WriteLine("Nombre de usuario no encontrado");
                return RedirectToAction("Ventas", "Ventas");
            }

        }
    }
}
