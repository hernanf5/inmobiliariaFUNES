using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public class TipoInmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTipoInmueble { get; set; }

        [Required]
        public string Nombre { get; set; } = "" ;

        public bool Activo { get; set; } = true;
    }
}