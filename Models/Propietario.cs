using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class Propietario
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int IdPropietario { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        [Display(Name = "DNI/CUIT")]
        public string DniCuit { get; set; } = "";

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";

        [Required, EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        public bool Activo { get; set; } = true;

        public override string ToString()
        {
            var res = $"{Nombre} {Apellido}";
            if (!string.IsNullOrEmpty(DniCuit))
            {
                res += $" ({DniCuit})";
            }
            return res;
        }
    }
}