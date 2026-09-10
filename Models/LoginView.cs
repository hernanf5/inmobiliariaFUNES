using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class LoginView
    {
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; } = "";
    }
}