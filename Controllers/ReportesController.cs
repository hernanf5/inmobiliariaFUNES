using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaFUNES.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioReserva repositorioReserva;
        private readonly ILogger<ReportesController> logger;

        public ReportesController(IRepositorioInmueble repoInmueble, IRepositorioReserva repoReserva, ILogger<ReportesController> logger)
        {
            this.repositorioInmueble = repoInmueble;
            this.repositorioReserva = repoReserva;
            this.logger = logger;
        }

        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reportes/MasReservados?dias=365
        public ActionResult MasReservados(int dias = 365)
        {
            try
            {
                ViewBag.Dias = dias;
                var lista = repositorioInmueble.ObtenerMasReservados(dias);
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en MasReservados");
                throw;
            }
        }
        // GET: Reportes/SinReservas?dias=60
        public ActionResult SinReservas(int dias = 60)
        {
            try
            {
                ViewBag.Dias = dias;
                var lista = repositorioInmueble.ObtenerSinReservas(dias);
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en SinReservas");
                throw;
            }
        }

        // GET: Reportes/TerminanPronto?dias=7
        public ActionResult TerminanPronto(int dias = 7)
        {
            try
            {
                ViewBag.Dias = dias;
                var lista = repositorioReserva.ObtenerQueTerminanEn(dias);
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en TerminanPronto");
                throw;
            }
        }
        // GET: Reportes/Disponibilidad
        public ActionResult Disponibilidad(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                if (fechaDesde.HasValue && fechaHasta.HasValue)
                {
                    if (fechaHasta.Value <= fechaDesde.Value)
                    {
                        ModelState.AddModelError(string.Empty, "La fecha hasta debe ser posterior a la fecha desde.");
                    }
                    else
                    {
                        ViewBag.Resultados = repositorioInmueble.ObtenerDisponiblesEntreFechas(fechaDesde.Value, fechaHasta.Value);
                    }
                    ViewBag.FechaDesde = fechaDesde.Value;
                    ViewBag.FechaHasta = fechaHasta.Value;
                }
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Disponibilidad");
                throw;
            }
        }
    }
}