using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models;

namespace PuntoVenta.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pagina = 1)
        {
            int totalRegistros = _context.UsuUsuario.Count();
            int registroPorPagina = 10;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registroPorPagina);

            // Obtener la lista de usuarios paginada
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id)
        {
            // Redirigir a la acción Edit con el ID del usuario
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var usuario = _context.UsuUsuario.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.UsuUsuario.Remove(usuario);
            _context.SaveChanges();

            // Redirigir a la acción Index después de eliminar el usuario
            return RedirectToAction(nameof(Index));
        }
    }
}
