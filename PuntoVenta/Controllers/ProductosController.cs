using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVenta.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using PuntoVenta.Models.Productos;
using PuntoVenta.Models.Ventas;

using Rotativa.AspNetCore;
using System.IO;
using System.Reflection.Metadata;
using iText.Kernel.Pdf;
using iText.Layout;
using System.IO;
using iText.Layout.Element;



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
        public IActionResult Productos(int pagina = 1)
        {
            int registrosPorPagina = 10;
            var totalProductos = _context.Productos.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / registrosPorPagina);
            var productos = _context.Productos
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            // Consultas adicionales para obtener los nombres de categorías y subcategorías
            var categoriaNombres = _context.Categorias.ToDictionary(c => c.IdCat, c => c.strNombreCategoria);
            var subcategoriaNombres = _context.SubCategorias.ToDictionary(s => s.IdSubCat, s => s.strNombreSubCategoria);

            ViewBag.CategoriaNombres = categoriaNombres;
            ViewBag.SubcategoriaNombres = subcategoriaNombres;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(productos);
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

        [HttpGet]
        public async Task<IActionResult> Ventas()
        {
            var userId = await ObtenerUserId();

            ViewBag.UserId = userId;

            Console.WriteLine($"Usuario obtenido correctamente: {userId}");
            // Carga las categorías para el dropdown
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCat", "strNombreCategoria");


            // Inicializa la lista de subcategorías y productos como vacía
            ViewBag.SubCategorias = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Productos = new SelectList(Enumerable.Empty<SelectListItem>());

            // Define la lista de categorías para el dropdown de categorías
            ViewBag.idProCatCategoria = new SelectList(_context.Categorias, "IdCat", "strNombreCategoria");

            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View("~/Views/Ventas/Ventas.cshtml");
        }


        private async Task<int?> ObtenerUserId()
        {
            Console.WriteLine("Controlador de ventas");
            var username = HttpContext.Session.GetString("Username");
            Console.WriteLine($"Usuario obtenido correctamente: {username}");

            var usuario = await _context.UsuUsuario.FirstOrDefaultAsync(u => u.strNombre == username);

            if (usuario != null)
            {
                return usuario.Id;
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetSubCategorias(int idCategoria)
        {
            var subcategorias = _context.SubCategorias.Where(sc => sc.idProCatCategoria == idCategoria)
                .Select(sc => new { value = sc.IdSubCat, text = sc.strNombreSubCategoria })
                .ToList();
            return Json(subcategorias);
        }

        public JsonResult GetProductos(int idSubCategoria)
        {
            var productos = _context.Productos.Where(p => p.idProCatSubCategoria == idSubCategoria)
                .Select(p => new { value = p.IdPro, text = p.StrNombrePro })
                .ToList();
            return Json(productos);
        }

        [HttpGet]
        public IActionResult ObtenerStockYPrecio(int productoId)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(p => p.IdPro == productoId);
                if (producto != null)
                {
                    var data = new
                    {
                        id = producto.IdPro, // Agregar el ID del producto a la respuesta
                        stock = producto.decStock,
                        precio = producto.curPrecio
                    };
                    return Json(data);
                }
                else
                {
                    return Json(new { error = "Producto no encontrado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener stock y precio del producto: {ErrorMessage}", ex.Message);
                return Json(new { error = "Error al obtener stock y precio del producto" });
            }
        }

        public IActionResult detalleVentas()
        {
            // Consultar todas las ventas con sus nombres de estado correspondientes
            var ventasConEstado = _context.VenVenta
                .Select(venta => new
                {
                    strFolio = venta.strFolio,
                    dtFecha = venta.dtFecha,
                    EstadoNombre = _context.venCatEstado.FirstOrDefault(e => e.idVenCatEstado == venta.idVenCatEstado) != null ? _context.venCatEstado.FirstOrDefault(e => e.idVenCatEstado == venta.idVenCatEstado).strNombre : "Estado no encontrado" // Obtener el nombre del estado por su ID
                })
                .ToList();

            ViewBag.Ventas = ventasConEstado;

            return View("~/Views/Ventas/detalleVentas.cshtml");
        }

        public IActionResult CancelarVenta(string strFolio)
        {
            // Buscar la venta por el folio proporcionado
            var venta = _context.VenVenta.FirstOrDefault(v => v.strFolio == strFolio);

            if (venta != null)
            {
                // Actualizar el estado de la venta a "cancelada" (suponiendo que el estado "cancelada" tiene un id de 2)
                venta.idVenCatEstado = 2; // Suponiendo que el estado "cancelada" tiene un id de 2
                _context.SaveChanges();
            }

            // Redirigir a la vista de ventas
            return RedirectToAction("detalleVentas");
        }


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
        public ActionResult Mostrar()
        {

            Console.WriteLine("Controlador de ventas");
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Ventas", "Ventas");
            }
            var model = new VenVenta();
            ViewBag.Username = username;
            return View(model);

        }






        [HttpGet]


        public IActionResult ObtenerUltimoIdVenta()
        {
            try
            {
                var ultimoIdVenta = _context.VenVenta.OrderByDescending(v => v.idVenVenta).Select(v => v.idVenVenta).FirstOrDefault();

                // Validar si se encontró algún ID de venta
                if (ultimoIdVenta != null)
                {
                    // Devolver el ID de la última venta encontrado
                    return Json(ultimoIdVenta);
                }
                else
                {
                    // Si no se encontró ningún ID de venta, devolver un valor predeterminado, como 0
                    return Json(0);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el último ID de venta: {ErrorMessage}", ex.Message);
                return Json(null); // Devolver un valor predeterminado o un mensaje de error
            }
        }


        [HttpPost]
        public async Task<IActionResult> FinalizarVenta([FromBody] VentaViewModel venta)
        {
            try
            {
                if (venta.Productos == null || venta.Productos.Count == 0)
                {
                    Console.WriteLine($"Folio recibido en el controlador: {venta.Folio}");
                    Console.WriteLine($"Cantidad de productos recibidos en el controlador: {(venta.Productos != null ? venta.Productos.Count.ToString() : "0")}"); // Verificar si Productos es nulo antes de contar
                    _logger.LogError("La lista de productos de la venta es nula o vacía.");
                    return RedirectToAction("detalleVentas"); // Redirigir a la vista de ventas si no se reciben productos
                }

                _logger.LogInformation("Recibida solicitud de finalizar venta. Folio: {Folio}, Cantidad de productos: {CantidadProductos}", venta.Folio, venta.Productos.Count);

                // Lógica para crear una nueva venta
                var nuevaVenta = new VenVenta
                {
                    idUsuUsuario = 1, // Valor fijo como mencionaste
                    dtFecha = DateTime.Now,
                    idVenCatEstado = 1, // Estado de venta fijo como mencionaste
                    strFolio = venta.Folio // Folio enviado desde la vista de ventas
                };

                // Agregar la venta a la base de datos y guardar
                _context.VenVenta.Add(nuevaVenta);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Venta creada correctamente. ID de venta: {IdVenta}", nuevaVenta.idVenVenta);

                // Obtenemos el ID de la venta recién creada
                int idVenta = nuevaVenta.idVenVenta;

                // Lógica para agregar los productos vendidos
                foreach (var productoVenta in venta.Productos)
                {
                    if (productoVenta == null)
                    {
                        _logger.LogError("ProductoVenta es null en la lista de Productos.");
                        continue;
                    }

                    // Asignamos el ID de la venta al producto
                    var nuevoProductoVenta = new VenVentaProducto
                    {
                        idVenVenta = idVenta,
                        idProProducto = productoVenta.IdProProducto,
                        decCantidad = productoVenta.Cantidad,
                        curTotal = productoVenta.Total
                    };
                    _context.VenVentaProductos.Add(nuevoProductoVenta);

                    // Actualizar el campo Stock en la tabla ProProductos
                    var producto = await _context.Productos.FindAsync(productoVenta.IdProProducto);
                    if (producto != null)
                    {
                        producto.decStock = productoVenta.Stock; // Actualizar el stock con el valor recibido
                        _context.Productos.Update(producto);
                    }
                }

                // Guardar todos los productos de la venta en la base de datos
                await _context.SaveChangesAsync();

                // Devolver una confirmación o redirigir a otra vista
                return RedirectToAction("detalleVentas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar la venta: {ErrorMessage}", ex.Message);
                // Manejar el error aquí
                return RedirectToAction("Ventas"); // Redirigir a la vista de ventas si ocurre un error
            }
        }





        [HttpGet]
        public IActionResult ObtenerIdProductoPorNombre(string nombreProducto)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(p => p.StrNombrePro == nombreProducto);
                if (producto != null)
                {
                    return Json(producto.IdPro);
                }
                else
                {
                    return Json(null); // Si no se encuentra el producto, devuelve null
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ID del producto por nombre: {ErrorMessage}", ex.Message);
                return Json(null); // Manejar errores devolviendo null
            }
        }

        //public IActionResult ImprimirTicket(string strFolio)
        //{
        //    Creamos un MemoryStream para almacenar el contenido del PDF
        //    using (var stream = new MemoryStream())
        //    {
        //        Inicializamos un nuevo documento PDF
        //       var writer = new PdfWriter(stream);
        //        var pdf = new PdfDocument(writer);
        //        iText.Layout.Document document = new iText.Layout.Document(pdf);

        //        try
        //        {
        //            Agregamos el texto "Hola" al documento
        //            Paragraph paragraph = new Paragraph("Hola");
        //            document.Add(paragraph);

        //            Cerramos el documento
        //            document.Close();

        //            Convertimos el MemoryStream en un array de bytes para descargar el PDF
        //            var pdfBytes = stream.ToArray();

        //            Descargamos el PDF como un archivo adjunto
        //            return File(pdfBytes, "application/pdf", "ticket.pdf");
        //        }
        //        catch (Exception ex)
        //        {
        //            Manejamos cualquier excepción que pueda ocurrir durante la generación del PDF
        //            En este caso, simplemente retornamos un mensaje de error
        //            return Content($"Error al generar el ticket: {ex.Message}");
        //        }
        //    }
        //}


        [HttpGet]
        public IActionResult ImprimirTicket(string strFolio)
        {
            // Creamos un MemoryStream para almacenar el contenido del PDF
            using (var stream = new MemoryStream())
            {
                // Inicializamos un nuevo documento PDF
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                try
                {
                    var venta = _context.VenVenta
                                       .Include(v => v.DetallesVentas)
                                       .ThenInclude(dv => dv.Producto)
                                       .FirstOrDefault(v => v.strFolio == strFolio);

                    // Si no se encuentra la venta, mostrar un mensaje de error
                    if (venta == null)
                    {
                        return Content($"No se encontró la venta con el folio {strFolio}");
                    }

                    // Agregar los datos de la venta al documento PDF
                    Paragraph titulo = new Paragraph("Detalle de la Venta");
                    document.Add(titulo);

                    // Agregar información de la venta al documento PDF
                    Paragraph folio = new Paragraph($"Folio: {venta.strFolio}");
                    document.Add(folio);

                    // Agregar fecha de la venta
                    Paragraph fecha = new Paragraph($"Fecha: {venta.dtFecha.ToShortDateString()}");
                    document.Add(fecha);

                    // Agregar detalles de los productos vendidos
                    foreach (var detalle in venta.DetallesVentas)
                    {
                        var producto = detalle.Producto as Products;  // Casting explícito

                        if (producto != null)
                        {
                            Paragraph productoInfo = new Paragraph($"Producto: {producto.StrNombrePro}, Cantidad: {detalle.decCantidad}, Subtotal: ${detalle.curTotal}");
                            document.Add(productoInfo);

                            // Agregar categoría y subcategoría del producto
                            var categoria = _context.Categorias.Find(producto.idProCatCategoria);
                            var subcategoria = _context.SubCategorias.Find(producto.idProCatSubCategoria);

                            if (categoria != null && subcategoria != null)
                            {
                                Paragraph categoriaInfo = new Paragraph($"Categoría: {categoria.strNombreCategoria}");
                                document.Add(categoriaInfo);

                                Paragraph subcategoriaInfo = new Paragraph($"Subcategoría: {subcategoria.strNombreSubCategoria}");
                                document.Add(subcategoriaInfo);
                            }
                        }
                    }

                    // Agregar total de la venta
                    Paragraph total = new Paragraph($"Total Venta: ${venta.DetallesVentas.Sum(d => d.curTotal)}");
                    document.Add(total);

                    // Agregar usuario que atendió
                    Paragraph atendidoPor = new Paragraph($"Atendido por: {venta.UsernameEmpleado}");
                    document.Add(atendidoPor);

                    // Cerrar el documento
                    document.Close();

                    // Convertir el MemoryStream en un array de bytes para descargar el PDF
                    var pdfBytes = stream.ToArray();

                    // Descargar el PDF como un archivo adjunto
                    return File(pdfBytes, "application/pdf", "ticket.pdf");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción que pueda ocurrir durante la generación del PDF
                    return Content($"Error al generar el ticket: {ex.Message}");
                }
            }
        }




    }

}
