﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using PuntoVenta.Helpers;
using Microsoft.Extensions.Logging; // Asegúrate de incluir esta directiva para ILogger

namespace PuntoVenta.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        // Constructor combinado con inyección de dependencias para ApplicationDbContext y ILogger
        public ProductosController(ApplicationDbContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Productos()
        {
            var proProductos = _context.Productos.ToList();

            // Consultas adicionales para obtener los nombres de categorías y subcategorías
            var categoriaNombres = _context.Categorias.ToDictionary(c => c.IdCat, c => c.strNombreCategoria);
            var subcategoriaNombres = _context.SubCategorias.ToDictionary(s => s.IdSubCat, s => s.strNombreSubCategoria);

            ViewBag.CategoriaNombres = categoriaNombres;
            ViewBag.SubcategoriaNombres = subcategoriaNombres;

            return View(proProductos);
        }




        public IActionResult Categorias()
        {
            var productos = _context.Productos.ToList();

            var productosConNombres = new List<ProductoConNombres>();

            foreach (var producto in productos)
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.IdCat == producto.idProCatCategoria);
                var subcategoria = _context.SubCategorias.FirstOrDefault(s => s.IdSubCat == producto.idProCatSubCategoria);

                if (categoria != null && subcategoria != null)
                {
                    var productoConNombres = new ProductoConNombres
                    {
                        Producto = producto,
                        CategoriaNombre = categoria.strNombreCategoria,
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
        public IActionResult Eliminar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                _context.Productos.Remove(producto);
                _context.SaveChanges();
                return RedirectToAction("Productos"); // Redirige a la acción "Productos" para actualizar la lista
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                return RedirectToAction("Productos"); // Redirige a la acción "Productos" si hay un error
            }
        }


        // Acción para mostrar la vista del formulario
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCat", "strNombreCategoria");
            ViewBag.SubCategorias = new SelectList(Enumerable.Empty<SelectListItem>(), "IdSubCat", "strNombreSubCategoria");
            return View(new Products());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Products producto, IFormFile blodImage)
        {
            if (ModelState.IsValid)
            {

                if (blodImage != null && blodImage.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await blodImage.CopyToAsync(stream);
                        producto.blodImage = stream.ToArray(); // Asegúrate de que esta propiedad se llame correctamente
                    }
                }
                else
                {
                    producto.blodImage = null; // Esto está bien sxi tu campo acepta nulos en la base de datos
                }


                try
                {
                 
                    _context.Add(producto);
                    _context.SaveChanges();
                    return RedirectToAction("Productos");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ha ocurrido un error al intentar crear un producto: {ErrorMessage}", ex.Message);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogError("Errores de validación al crear producto: {ValidationErrors}", errors);
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCat", "strNombreCategoria", producto.idProCatCategoria);
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias.Where(sc => sc.idProCatCategoria == producto.idProCatCategoria), "IdSubCat", "strNombreSubCategoria", producto.idProCatSubCategoria);

            return View(producto);
        }



        [HttpGet]
        public IActionResult GetSubcategoriasPorCategoria(int categoriaId)
        {
            try
            {
                var subcategorias = _context.SubCategorias
                    .Where(sc => sc.idProCatCategoria == categoriaId)
                    .Select(sc => new { value = sc.IdSubCat, text = sc.strNombreSubCategoria })
                    .ToList();

                return Json(subcategorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener subcategorías por categoría: {ErrorMessage}", ex.Message);
                throw; // Opcional: Puedes manejar la excepción de otra manera si lo prefieres
            }
        }

    }
}
