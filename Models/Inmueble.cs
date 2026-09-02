using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace inmobiliariaFUNES.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }

        [Required]
        public string Direccion { get; set; } = "";

        [Required]
        public int Cupo { get; set; }

        [Required]
        [Display(Name = "Precio por día")]
        public decimal PrecioPorDia { get; set; }

        [Required]
        [Display(Name = "% Reserva")]
        public decimal PorcentajeReserva { get; set; } = 30;

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        [Required]
        [Display(Name = "Propietario")]
        public int IdPropietario { get; set; }

        [Required]
        [Display(Name = "Tipo de inmueble")]
        public int IdTipoInmueble { get; set; }

        public string Estado { get; set; } = "Disponible";

        // Navegación: se completan con un JOIN en el repositorio, no vienen del formulario.
        public Propietario? Propietario { get; set; }
        public TipoInmueble? TipoInmueble { get; set; }
        public IList<ImagenInmueble> Imagenes { get; set; } = new List<ImagenInmueble>();

        public IFormFile? PortadaFile { get; set; }
        public IList<IFormFile>? GaleriaFiles { get; set; }
    }
}