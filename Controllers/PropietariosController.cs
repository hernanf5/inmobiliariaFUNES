using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaFUNES.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorio;
        private readonly ILogger<PropietariosController> logger;

        public PropietariosController(IRepositorioPropietario repo, ILogger<PropietariosController> logger)
        {
            this.repositorio = repo;
            this.logger = logger;
        }

        // GET: Propietarios
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 5;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamaño == 0 ? total/tamaño : total / tamaño + 1;
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

        //GET: Propietarios/Details/5
        public IActionResult Details(int id)
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

        //GET: Propietarios/Obtener/5
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

        // Get: Propietarios/Busqueda
        public IActionResult Busqueda()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Busqueda");
                throw;
            }
        }

        //GET: Propietarios/Buscar/algo
        [Route("[controller]/Buscar.{q}", Name = "BuscarPropietarios")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorNombre(q);
                return Json(new {Datos = res});
            }
            catch (Exception ex)
            {
                
                return Json(new { Error = ex.Message});
            }
        }

        //GET: Propietarios/Create
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

        //POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(propietario);
                    TempData["Id"] = propietario.IdPropietario;
                    TempData["Mensaje"] = "Prpietario creado correctamente";

                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        //GET: Propietarios/Edit/5
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

        //POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario entidad)
        {
            try
            {
                var p = repositorio.ObtenerPorId(id);
                if(p == null)
                    return NotFound();
                if (!ModelState.IsValid)
                    return View(entidad);
                
                p.Nombre = entidad.Nombre;
                p.Apellido = entidad.Apellido;
                p.DniCuit = entidad.DniCuit;
                p.Email = entidad.Email;
                p.Telefono = entidad.Telefono;
                repositorio.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en edit");
                throw;
            }
        }

        //GET: Propietarios/Eliminar/5
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

        //POST: Propietarios/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Propietario entidad)
        {
            try
            {
                var p = repositorio.ObtenerPorId(id);
                if (p == null)
                    return NotFound();
                repositorio.Baja(p);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
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