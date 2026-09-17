using System;
using System.IO;
using System.Threading.Tasks;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace inmobiliariaFUNES.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<InmueblesController> logger;

        public InmueblesController(
            IRepositorioInmueble repo,
            IRepositorioPropietario repoPropietario,
            IRepositorioTipoInmueble repoTipoInmueble,
            IWebHostEnvironment environment,
            ILogger<InmueblesController> logger)
        {
            this.repositorio = repo;
            this.repositorioPropietario = repoPropietario;
            this.repositorioTipoInmueble = repoTipoInmueble;
            this.environment = environment;
            this.logger = logger;
        }

        // GET: Inmuebles
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1, string? estado = null, int? idPropietario = null)
        {
            try
            {
                var tamaño = 10;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño, estado, idPropietario);
                ViewBag.Pagina = pagina;
                ViewBag.EstadoFiltro = estado;
                ViewBag.IdPropietarioFiltro = idPropietario;
                if (idPropietario.HasValue)
                {
                    var propietario = repositorioPropietario.ObtenerPorId(idPropietario.Value);
                    ViewBag.NombrePropietarioFiltro = propietario?.ToString();
                }
                var total = repositorio.ObtenerCantidad(estado, idPropietario);
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index");
                throw;
            }
        }

        // GET: Inmuebles/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Details");
                throw;
            }
        }

        private void CargarListasDesplegables(int? idPropietarioSeleccionado = null, int? idTipoSeleccionado = null)
        {
            if (idPropietarioSeleccionado.HasValue)
            {
                var propietario = repositorioPropietario.ObtenerPorId(idPropietarioSeleccionado.Value);
                ViewBag.PropietarioSeleccionadoTexto = propietario?.ToString();
            }

            var tipos = repositorioTipoInmueble.ObtenerLista(1, int.MaxValue);
            ViewBag.Tipos = new SelectList(tipos, nameof(TipoInmueble.IdTipoInmueble), nameof(TipoInmueble.Nombre), idTipoSeleccionado);
        }

        // GET: Inmuebles/Buscar/q
        // [Route("[controller]/Buscar/{q}")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorDireccion(q);
                return Json(new { datos = res.Select(i => new { id = i.IdInmueble, texto = i.Direccion }) });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            try
            {
                CargarListasDesplegables();
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Inmueble inmueble)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarListasDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                    return View(inmueble);
                }

                try
                {
                    repositorio.Alta(inmueble);
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Out of range"))
                {
                    ModelState.AddModelError(string.Empty, "Las coordenadas ingresadas no son válidas.");
                    CargarListasDesplegables(inmueble.IdPropietario, inmueble.IdTipoInmueble);
                    return View(inmueble);
                }

                string carpetaInmueble = Path.Combine(environment.WebRootPath, "uploads", "inmuebles", inmueble.IdInmueble.ToString());
                Directory.CreateDirectory(carpetaInmueble);

                if (inmueble.PortadaFile != null && inmueble.PortadaFile.Length > 0)
                {
                    var urlPortada = await GuardarArchivo(inmueble.PortadaFile, carpetaInmueble, inmueble.IdInmueble);
                    repositorio.AgregarImagen(new ImagenInmueble
                    {
                        IdInmueble = inmueble.IdInmueble,
                        Url = urlPortada,
                        EsPortada = true,
                    });
                }

                if (inmueble.GaleriaFiles != null)
                {
                    foreach (var archivo in inmueble.GaleriaFiles)
                    {
                        if (archivo.Length > 0)
                        {
                            var url = await GuardarArchivo(archivo, carpetaInmueble, inmueble.IdInmueble);
                            repositorio.AgregarImagen(new ImagenInmueble
                            {
                                IdInmueble = inmueble.IdInmueble,
                                Url = url,
                                EsPortada = false,
                            });
                        }
                    }
                }

                TempData["Id"] = inmueble.IdInmueble;
                TempData["Mensaje"] = "Inmueble creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        private async Task<string> GuardarArchivo(IFormFile archivo, string carpetaFisica, int idInmueble)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpetaFisica, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/inmuebles/{idInmueble}/{nombreArchivo}";
        }

        // GET: Inmuebles/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                CargarListasDesplegables(entidad.IdPropietario, entidad.IdTipoInmueble);
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Inmueble entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    CargarListasDesplegables(entidad.IdPropietario, entidad.IdTipoInmueble);
                    return View(entidad);
                }

                i.Direccion = entidad.Direccion;
                i.Cupo = entidad.Cupo;
                i.PrecioPorDia = entidad.PrecioPorDia;
                i.PorcentajeReserva = entidad.PorcentajeReserva;
                i.Latitud = entidad.Latitud;
                i.Longitud = entidad.Longitud;
                i.IdPropietario = entidad.IdPropietario;
                i.IdTipoInmueble = entidad.IdTipoInmueble;
                try
                {
                    repositorio.Modificacion(i);
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Out of range"))
                {
                    ModelState.AddModelError(string.Empty, "Las coordenadas ingresadas no son válidas.");
                    CargarListasDesplegables(i.IdPropietario, i.IdTipoInmueble);
                    return View(i);
                }

                string carpetaInmueble = Path.Combine(environment.WebRootPath, "uploads", "inmuebles", id.ToString());
                Directory.CreateDirectory(carpetaInmueble);

                if (entidad.PortadaFile != null && entidad.PortadaFile.Length > 0)
                {
                    var urlPortada = await GuardarArchivo(entidad.PortadaFile, carpetaInmueble, id);
                    repositorio.AgregarImagen(new ImagenInmueble
                    {
                        IdInmueble = id,
                        Url = urlPortada,
                        EsPortada = true,
                    });
                }

                if (entidad.GaleriaFiles != null)
                {
                    foreach (var archivo in entidad.GaleriaFiles)
                    {
                        if (archivo.Length > 0)
                        {
                            var url = await GuardarArchivo(archivo, carpetaInmueble, id);
                            repositorio.AgregarImagen(new ImagenInmueble
                            {
                                IdInmueble = id,
                                Url = url,
                                EsPortada = false,
                            });
                        }
                    }
                }

                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

                // GET: Inmuebles/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        // POST: Inmuebles/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Inmueble entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null)
                    return NotFound();

                repositorio.Baja(i);
                TempData["Mensaje"] = "El inmueble fue suspendido correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        // POST: Inmuebles/Reactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Reactivar(int id)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null)
                    return NotFound();

                repositorio.Reactivar(i);
                TempData["Mensaje"] = "El inmueble fue reactivado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Reactivar");
                throw;
            }
        }
    }
}