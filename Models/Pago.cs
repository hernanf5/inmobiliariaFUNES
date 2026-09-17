using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "Código")]
        public int IdPago { get; set; }

        [Required]
        [Display(Name = "Reserva")]
        public int IdReserva { get; set; }

        [Required]
        public string Concepto { get; set; } = "";

        [Required]
        [Display(Name = "Fecha de pago")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; } = DateTime.Today;

        [Required]
        public decimal Importe { get; set; }

        public string Estado { get; set; } = "Activo";

        public int? IdUsuarioCreador { get; set; }
        public int? IdUsuarioAnulador { get; set; }

        public string? NombreUsuarioCreador { get; set; }
        public string? NombreUsuarioAnulador { get; set; }
    }
}