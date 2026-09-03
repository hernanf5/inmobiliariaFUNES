using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Código")]
        public int IdReserva { get; set; }

        [Required]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [Required]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        public int? IdReservaOrigen { get; set; }

        [Required]
        [Display(Name = "Monto por día")]
        public decimal MontoPorDia { get; set; }

        [Required]
        [Display(Name = "Fecha desde")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }

        [Required]
        [Display(Name = "Fecha hasta")]
        [DataType(DataType.Date)]
        public DateTime FechaHastaOriginal { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaTerminacion { get; set; }

        public decimal? Multa { get; set; }

        public string Estado { get; set; } = "Vigente";

        public int? IdUsuarioCreador { get; set; }
        public int? IdUsuarioTerminador { get; set; }

        // Navegación: se completan con JOIN en el repositorio.
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
    }
}