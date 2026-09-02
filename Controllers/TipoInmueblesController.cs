using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaFUNES.Controllers
{
    public class TipoInmueblesController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly ILogger<TipoInmueblesController> logger;

        public TipoInmueblesController(IRepositorioTipoInmueble repo, ILogger<TipoInmueblesController> logger)
        {
            this.repositorio = repo;
            this.logger = logger;
        }

        // GET: TipoInmuebles
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 10;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
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

                // GET: TipoInmuebles/Details/5
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

        // GET: TipoInmuebles/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        // POST: TipoInmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoInmueble tipoInmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(tipoInmueble);
                    TempData["Id"] = tipoInmueble.IdTipoInmueble;
                    TempData["Mensaje"] = "Tipo de inmueble creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(tipoInmueble);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

                // GET: TipoInmuebles/Edit/5
        public ActionResult Edit(int id)
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
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // POST: TipoInmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TipoInmueble entidad)
        {
            try
            {
                var t = repositorio.ObtenerPorId(id);
                if (t == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return View(entidad);

                t.Nombre = entidad.Nombre;
                repositorio.Modificacion(t);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // GET: TipoInmuebles/Eliminar/5
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

        // POST: TipoInmuebles/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, TipoInmueble entidad)
        {
            try
            {
                var t = repositorio.ObtenerPorId(id);
                if (t == null)
                    return NotFound();

                repositorio.Baja(t);
                TempData["Mensaje"] = "Baja realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }
    }
}