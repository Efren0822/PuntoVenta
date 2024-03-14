﻿using Microsoft.AspNetCore.Mvc;
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
                var subcategoria = subcategorias.FirstOrDefault(s => s.IdSubCat == producto.idProCatSubCategoria && s.idProCatCategoria == producto.idProCatCategoria);

                if (subcategoria != null)
                {
                    var productoConNombres = new ProductoConNombres
                    {
                        Producto = producto,
                        CategoriaNombre = subcategoria.idProCatCategoria.ToString(), // Aquí puedes asignar el nombre de la categoría si lo deseas
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



        public IActionResult Crear()
        {
            // Preparar las listas para categorías y subcategorías
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Products producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.idProCatCategoria);
            ViewBag.SubCategorias = new SelectList(_context.SubCategorias, "Id", "Nombre", producto.idProCatSubCategoria);
            return View(producto);
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
    }
}