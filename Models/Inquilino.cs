using System;
using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInquilino { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public string Dni { get; set; } = "";

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public bool Activo { get; set; } = true;

        public override string ToString()
        {
            var res = $"{Nombre} {Apellido}";
            if (!string.IsNullOrEmpty(Dni))
            {
                res += $" ({Dni})";
            }
            return res;
        }
    }
}