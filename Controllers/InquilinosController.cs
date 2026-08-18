using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaFUNES.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;
        private readonly ILogger<InquilinosController> logger;

        public InquilinosController(IRepositorioInquilino repo, ILogger<InquilinosController> logger)
        {
            this.repositorio = repo;
            this.logger = logger;
        }

        // GET: Inquilinos
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 5;
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

        // GET: Inquilinos/Details/5
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

        // GET: Inquilinos/Obtener/5
        public IActionResult Obtener(int id)
        {
            try
            {
                var res = repositorio.ObtenerPorId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Inquilinos/Create
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

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(inquilino);
                    TempData["Id"] = inquilino.IdInquilino;
                    TempData["Mensaje"] = "Inquilino creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(inquilino);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        // GET: Inquilinos/Edit/5
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

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return View(entidad);

                i.Dni = entidad.Dni;
                i.Nombre = entidad.Nombre;
                i.Apellido = entidad.Apellido;
                i.Telefono = entidad.Telefono;
                i.Email = entidad.Email;
                repositorio.Modificacion(i);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // GET: Inquilinos/Eliminar/5
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

        // POST: Inquilinos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Inquilino entidad)
        {
            try
            {
                var i = repositorio.ObtenerPorId(id);
                if (i == null)
                    return NotFound();

                repositorio.Baja(i);
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