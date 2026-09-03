using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliariaFUNES.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly ILogger<ReservasController> logger;

        public ReservasController(
            IRepositorioReserva repo,
            IRepositorioInquilino repoInquilino,
            IRepositorioInmueble repoInmueble,
            ILogger<ReservasController> logger)
        {
            this.repositorio = repo;
            this.repositorioInquilino = repoInquilino;
            this.repositorioInmueble = repoInmueble;
            this.logger = logger;
        }

        // GET: Reservas
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
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index");
                throw;
            }
        }

        private void CargarListasDesplegables(int? idInquilinoSeleccionado = null, int? idInmuebleSeleccionado = null)
        {
            var inquilinos = repositorioInquilino.ObtenerLista(1, int.MaxValue);
            ViewBag.Inquilinos = new SelectList(inquilinos, nameof(Inquilino.IdInquilino), null, idInquilinoSeleccionado);

            var inmuebles = repositorioInmueble.ObtenerLista(1, int.MaxValue);
            ViewBag.Inmuebles = new SelectList(inmuebles, nameof(Inmueble.IdInmueble), nameof(Inmueble.Direccion), idInmuebleSeleccionado);
        }

        // GET: Reservas/Details/5
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

        // GET: Reservas/Create
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

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva reserva)
        {
            try
            {
                if (reserva.FechaHastaOriginal <= reserva.FechaDesde)
                {
                    ModelState.AddModelError(nameof(Reserva.FechaHastaOriginal), "La fecha hasta debe ser posterior a la fecha desde.");
                }

                if (!ModelState.IsValid)
                {
                    CargarListasDesplegables(reserva.IdInquilino, reserva.IdInmueble);
                    return View(reserva);
                }

                try
                {
                    repositorio.Alta(reserva);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    CargarListasDesplegables(reserva.IdInquilino, reserva.IdInmueble);
                    return View(reserva);
                }

                TempData["Id"] = reserva.IdReserva;
                TempData["Mensaje"] = "Reserva creada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        // GET: Reservas/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                if (entidad.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden editar reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }
                CargarListasDesplegables(entidad.IdInquilino, entidad.IdInmueble);
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Reserva entidad)
        {
            try
            {
                var r = repositorio.ObtenerPorId(id);
                if (r == null)
                    return NotFound();
                if (r.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden editar reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }

                if (entidad.FechaHastaOriginal <= entidad.FechaDesde)
                {
                    ModelState.AddModelError(nameof(Reserva.FechaHastaOriginal), "La fecha hasta debe ser posterior a la fecha desde.");
                }

                if (!ModelState.IsValid)
                {
                    CargarListasDesplegables(entidad.IdInquilino, entidad.IdInmueble);
                    return View(entidad);
                }

                r.IdInquilino = entidad.IdInquilino;
                r.IdInmueble = entidad.IdInmueble;
                r.MontoPorDia = entidad.MontoPorDia;
                r.FechaDesde = entidad.FechaDesde;
                r.FechaHastaOriginal = entidad.FechaHastaOriginal;

                try
                {
                    repositorio.Modificacion(r);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    CargarListasDesplegables(entidad.IdInquilino, entidad.IdInmueble);
                    return View(entidad);
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

        // GET: Reservas/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                if (entidad.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden cancelar reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        // POST: Reservas/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Reserva entidad)
        {
            try
            {
                var r = repositorio.ObtenerPorId(id);
                if (r == null)
                    return NotFound();
                if (r.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden cancelar reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }

                repositorio.Baja(r);
                TempData["Mensaje"] = "Reserva cancelada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        private decimal CalcularMulta(Reserva reserva, DateTime fechaTerminacion)
        {
            var duracionOriginalDias = (reserva.FechaHastaOriginal - reserva.FechaDesde).TotalDays;
            var diasCumplidos = (fechaTerminacion - reserva.FechaDesde).TotalDays;

            var diasRestantesOriginales = duracionOriginalDias - diasCumplidos;
            var montoRestante = (decimal)diasRestantesOriginales * reserva.MontoPorDia;

            bool cumplioMenosDeLaMitad = diasCumplidos < (duracionOriginalDias / 2);
            decimal porcentajeMulta = cumplioMenosDeLaMitad ? 0.50m : 0.25m;

            return Math.Round(montoRestante * porcentajeMulta, 2);
        }

        // GET: Reservas/Terminar/5
        public ActionResult Terminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                if (entidad.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden terminar anticipadamente reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }

                var fechaTerminacion = DateTime.Today;
                ViewBag.FechaTerminacion = fechaTerminacion;
                ViewBag.MultaCalculada = CalcularMulta(entidad, fechaTerminacion);
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Terminar");
                throw;
            }
        }

        // POST: Reservas/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Terminar(int id, DateTime fechaTerminacion)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                if (entidad.Estado != "Vigente")
                {
                    TempData["Error"] = "Solo se pueden terminar anticipadamente reservas vigentes.";
                    return RedirectToAction(nameof(Index));
                }
                if (fechaTerminacion < entidad.FechaDesde || fechaTerminacion > entidad.FechaHastaOriginal)
                {
                    TempData["Error"] = "La fecha de terminación debe estar dentro del período original de la reserva.";
                    return RedirectToAction(nameof(Terminar), new { id });
                }

                var multa = CalcularMulta(entidad, fechaTerminacion);
                repositorio.Terminar(entidad, fechaTerminacion, multa);

                TempData["Mensaje"] = $"Reserva terminada anticipadamente. Multa calculada: ${multa:N2}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Terminar");
                throw;
            }
        }
    }
}

