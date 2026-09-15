using System;
using System.Security.Claims;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaFUNES.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioReserva repositorioReserva;
        private readonly ILogger<PagosController> logger;

        public PagosController(IRepositorioPago repo, IRepositorioReserva repoReserva, ILogger<PagosController> logger)
        {
            this.repositorio = repo;
            this.repositorioReserva = repoReserva;
            this.logger = logger;
        }

        private int ObtenerIdUsuarioLogueado()
        {
            var idTexto = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idTexto, out var id) ? id : 0;
        }
        // GET: Pagos?idReserva=5
        public ActionResult Index(int idReserva)
        {
            try
            {
                var reserva = repositorioReserva.ObtenerPorId(idReserva);
                if (reserva == null)
                    return NotFound();

                var pagos = repositorio.ObtenerPorReserva(idReserva);
                ViewBag.Reserva = reserva;
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(pagos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index");
                throw;
            }
        }
        // GET: Pagos/Create?idReserva=5
        public ActionResult Create(int idReserva)
        {
            try
            {
                var reserva = repositorioReserva.ObtenerPorId(idReserva);
                if (reserva == null)
                    return NotFound();

                ViewBag.Reserva = reserva;
                return View(new Pago { IdReserva = idReserva, FechaPago = DateTime.Today });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago pago)
        {
            try
            {
                var reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
                if (reserva == null)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    ViewBag.Reserva = reserva;
                    return View(pago);
                }

                pago.IdUsuarioCreador = ObtenerIdUsuarioLogueado();
                repositorio.Alta(pago);

                TempData["Mensaje"] = "Pago registrado correctamente";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }
        // GET: Pagos/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();
                if (pago.Estado != "Activo")
                {
                    TempData["Error"] = "No se puede editar un pago anulado.";
                    return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
                }

                ViewBag.Reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
                return View(pago);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string concepto)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();
                if (pago.Estado != "Activo")
                {
                    TempData["Error"] = "No se puede editar un pago anulado.";
                    return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
                }

                pago.Concepto = concepto;
                repositorio.Modificacion(pago);

                TempData["Mensaje"] = "Concepto actualizado correctamente";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }
        // GET: Pagos/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();
                if (pago.Estado != "Activo")
                {
                    TempData["Error"] = "Este pago ya está anulado.";
                    return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
                }

                ViewBag.Reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
                return View(pago);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        // POST: Pagos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Pago entidad)
        {
            try
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();
                if (pago.Estado != "Activo")
                {
                    TempData["Error"] = "Este pago ya está anulado.";
                    return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
                }

                pago.IdUsuarioAnulador = ObtenerIdUsuarioLogueado();
                repositorio.Baja(pago);

                TempData["Mensaje"] = "Pago anulado correctamente";
                return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }
    }
}