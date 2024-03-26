using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PuntoVenta.ViewModels;
using PuntoVenta.Helpers;

namespace PuntoVenta.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(ApplicationDbContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int pagina = 1)
        {
            int totalRegistros = _context.UsuUsuario.Count();
            int registroPorPagina = 10;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);


            List<UsuarioViewModel> usuarios = _context.UsuUsuario
                .Include(u => u.Estado)
                .Include(u => u.TipoUsuario)
                .Select(u => new UsuarioViewModel
                {
                    Id = u.Id,
                    strNombre = u.strNombre,
                    NombreEstado = u.Estado.strNombreEstado,
                    NombreTipoUsuario = u.TipoUsuario.strTipoUsuario
                })
                .Skip((pagina - 1) * registroPorPagina)
                .Take(registroPorPagina)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(usuarios);
        }



        private int ObtenerIdEstado()
        {
            var estadoDefecto = _context.UsuCatEstado.FirstOrDefault(e => e.strNombreEstado.Equals("Activo"));
            if (estadoDefecto == null)
            {
                throw new InvalidOperationException("No se encontro el estado por defecto");
            }
            return estadoDefecto.Id;

        }

        private int ObtenerIdTipoUsuario()
        {
            var tipoUsuarioD = _context.UsuCatTipoUsuario.FirstOrDefault(t => t.strTipoUsuario.Equals("Usuario Normal"));
            if (tipoUsuarioD == null)
            {
                throw new InvalidOperationException("No se encontro el estado por defecto");
            }
            return tipoUsuarioD.Id;
        }


       
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string username = model.Username.ToLower();
            var user = _context.UsuUsuario.Any(u => u.strNombre == username && u.strPassword == model.Password);

            if (user)
            {
                TempData["Mensaje"] = "¡Bienvenido, " + model.Username + "!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Nombre de usuario y/o contraseña incorrectos.");
                return View(model);
            }
        }

        

        public IActionResult Borrar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _context.UsuUsuario.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Borrar")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarBorrar(int id)
        {
            var usuario = _context.UsuUsuario.Find(id);
            _context.UsuUsuario.Remove(usuario);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.UsuUsuario.Any(e => e.Id == id);
        }



        public ActionResult Create()
        {
            ViewBag.Estado = _context.UsuCatEstado
        .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
        .ToList();
            ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                        .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                        .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            string errorMessage = "";

            if (ModelState.IsValid)
            {
                if (usuario.IdUsuCatEstado == 0 && usuario.IdUsuCatTipoUsuario == 0)
                {
                    ModelState.AddModelError("EstadoId", "Debe seleccionar un estado.");
                    ModelState.AddModelError("TipoUsuario", "Debe seleccionar un tipo de usuario.");
                    ViewBag.Estado = _context.UsuCatEstado
                        .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
                        .ToList();
                    ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                                .ToList();
                    return View(usuario);
                }

                usuario.strPassword = Encriptacion.Encriptar(usuario.strPassword);
            }
            else
            {
                errorMessage = "ModelState.IsValid fue falso. Los errores de validación son los siguientes:";
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errorMessage += $" {error.ErrorMessage}";
                    }
                }
            }

            try
            {
                
                _context.UsuUsuario.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errorMessage += $" Ocurrió un error al guardar el usuario en la base de datos: {ex.Message}.";
            }

            ModelState.AddModelError("", $"Ocurrió un error: {errorMessage} Por favor, inténtelo de nuevo más tarde.");

            ViewBag.Estado = _context.UsuCatEstado
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
                .ToList();
            ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                        .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                        .ToList();
            return View(usuario);
        }


        public ActionResult CreateL()
        {
            ViewBag.Estado = _context.UsuCatEstado
        .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
        .ToList();
            ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                        .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                        .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateL(Usuario usuario)
        {
            string errorMessage = "";

            if (ModelState.IsValid)
            {
                if (usuario.IdUsuCatEstado == 0 && usuario.IdUsuCatTipoUsuario == 0)
                {
                    ModelState.AddModelError("EstadoId", "Debe seleccionar un estado.");
                    ModelState.AddModelError("TipoUsuario", "Debe seleccionar un tipo de usuario.");
                    ViewBag.Estado = _context.UsuCatEstado
                        .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
                        .ToList();
                    ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                                .ToList();
                    return View(usuario);
                }

                usuario.strPassword = Encriptacion.Encriptar(usuario.strPassword);
            }
            else
            {
                errorMessage = "ModelState.IsValid fue falso. Los errores de validación son los siguientes:";
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errorMessage += $" {error.ErrorMessage}";
                    }
                }
            }

            try
            {

                _context.UsuUsuario.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errorMessage += $" Ocurrió un error al guardar el usuario en la base de datos: {ex.Message}.";
            }

            ModelState.AddModelError("", $"Ocurrió un error: {errorMessage} Por favor, inténtelo de nuevo más tarde.");

            ViewBag.Estado = _context.UsuCatEstado
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.strNombreEstado })
                .ToList();
            ViewBag.TipoUsuario = _context.UsuCatTipoUsuario
                        .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.strTipoUsuario })
                        .ToList();
            return View(usuario);
        }



        public IActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _context.UsuUsuario
                .Include(u => u.Estado)
                .Include(u => u.TipoUsuario)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        public IActionResult Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _context.UsuUsuario.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener la lista de estados
            var estados = _context.UsuCatEstado
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.strNombreEstado
                })
                .ToList();

            // Obtener la lista de tipos de usuario
            var tiposUsuario = _context.UsuCatTipoUsuario
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.strTipoUsuario
                })
                .ToList();

            // Pasar las listas a la vista
            ViewBag.Estados = estados;
            ViewBag.TiposUsuario = tiposUsuario;

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, [Bind("Id,strNombre,IdUsuCatEstado,IdUsuCatTipoUsuario")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar los cambios: " + ex.Message);
                    Console.WriteLine("error: " + ex.Message);
                    ViewBag.Estados = _context.UsuCatEstado
                        .Select(e => new SelectListItem
                        {
                            Value = e.Id.ToString(),
                            Text = e.strNombreEstado
                        })
                        .ToList();

                    ViewBag.TiposUsuario = _context.UsuCatTipoUsuario
                        .Select(t => new SelectListItem
                        {
                            Value = t.Id.ToString(),
                            Text = t.strTipoUsuario
                        })
                        .ToList();

                    return View(usuario);
                }
            }

            // Si la validación falla, obtener nuevamente las listas de estados y tipos de usuario y pasarlas a la vista
            ViewBag.Estados = _context.UsuCatEstado
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.strNombreEstado
                })
                .ToList();

            ViewBag.TiposUsuario = _context.UsuCatTipoUsuario
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.strTipoUsuario
                })
                .ToList();

            return View(usuario);
        }







    }

}

