using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using PuntoVenta.Helpers;

namespace PuntoVenta.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        public IActionResult Productos()
        {
            var proProductos = _context.Productos
                .Include(p => p.Categoria) 
                .Include(p => p.SubCategoria) 
                .ToList();

            return View(proProductos);
        }

        public IActionResult Categorias()
        {
            var proProductos = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .ToList();

            var subcategorias = _context.SubCategorias.ToList();

            var productosConNombres = new List<ProductoConNombres>();

            foreach (var producto in proProductos)
            {
                var subcategoriasProducto = subcategorias.Where(s => s.IdSubCat == producto.idProCatSubCategoria && s.idProCatCategoria == producto.idProCatCategoria).ToList();

                foreach (var subcategoria in subcategoriasProducto)
                {
                    var productoConNombres = new ProductoConNombres
                    {
                        Producto = producto,
                        CategoriaNombre = producto.Categoria.strNombreCategoria,
                        SubCategoriaNombre = subcategoria.strNombreSubCategoria
                    };
                    productosConNombres.Add(productoConNombres);
                }
            }

            return View(productosConNombres);
        }



        public class ProductoConNombres
        {
            public Products Producto { get; set; }
            public string CategoriaNombre { get; set; }
            public string SubCategoriaNombre { get; set; }
        }

        

        public IActionResult Editar(int? id)
        {
            if (id == null || !_context.Productos.Any(p => p.IdPro == id))
            {
                return NotFound();
            }

            var producto = _context.Productos.Find(id);
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.idProCatCategoria);
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias, "Id", "Nombre", producto.idProCatSubCategoria);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Products producto)
        {
            if (id != producto.IdPro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdPro))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.idProCatCategoria);
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias, "Id", "Nombre", producto.idProCatSubCategoria);
            return View(producto);
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null || !_context.Productos.Any(p => p.IdPro == id))
            {
                return NotFound();
            }

            var producto = _context.Productos
                .Include(c => c.Categoria) // Asegúrate de ajustar esto según la estructura de tu modelo
                .Include(a => a.SubCategoria) // Asegúrate de ajustar esto según la estructura de tu modelo
                .FirstOrDefault(m => m.IdPro == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id)
        {
            var producto = _context.Productos.Find(id);
            _context.Productos.Remove(producto);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdPro == id);
        }


        // Acción para mostrar la vista del formulario
        [HttpGet]
        public IActionResult Crear()
        {
            // Obtener las categorías de la base de datos y asegurarse de que no sean nulas
            var categorias = _context.Categorias.ToList();
            if (categorias == null || !categorias.Any())
            {
                // Manejar el caso cuando no hay categorías, tal vez redirigir o mostrar un mensaje
                // Por ahora, asignaremos un SelectList vacío para evitar errores
                ViewBag.Categorias = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            else 
            {
                // Si hay categorías, crear el SelectList con ellas
                ViewBag.Categorias = new SelectList(categorias, "IdCat", "strNombreCategoria");
                ViewBag.SubCategorias = new SelectList(Enumerable.Empty<SelectListItem>()); // Inicializa vacío.


            }

            // Para las subcategorías, inicialmente tendremos una lista vacía porque dependen de la categoría seleccionada
            ViewBag.SubCategorias = new SelectList(Enumerable.Empty<SelectListItem>());

            // Pasar una instancia nueva de Products para evitar referencias nulas
            return View(new Products());
        }



        // Acción para procesar los datos del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Products producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                _context.SaveChanges();
                return RedirectToAction("Productos"); // Asegúrate de redirigir a la vista o acción correcta.
            }

            // Recargar ViewBag en caso de un error para mantener el formulario
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCat", "strNombreCategoria", producto.idProCatCategoria);
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias, "IdSubCat", "strNombreSubCategoria", producto.idProCatSubCategoria);
            return View(producto);
        }


        // Acción para obtener subcategorías en base a la categoría seleccionada, usando AJAX
        [HttpGet]
        public IActionResult GetSubcategoriasPorCategoria(int categoriaId)
        {
            var subcategorias = _context.SubCategorias
                                        .Where(sc => sc.idProCatCategoria == categoriaId)
                                        .Select(sc => new { value = sc.IdSubCat, text = sc.strNombreSubCategoria })
                                        .ToList();

            return Json(subcategorias);
        }

    }
}
