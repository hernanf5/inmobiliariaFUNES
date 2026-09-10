using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace inmobiliariaFUNES.Models
{
    public enum RolUsuario
    {
        Administrador = 1,
        Empleado = 2,
    }

    public class Usuario
    {
        [Key]
        [Display(Name = "Código")]
        public int IdUsuario { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; } = "";

        public int Rol { get; set; }

        public bool Activo { get; set; } = true;

        public string RolNombre => Rol > 0 ? ((RolUsuario)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
        {
            var roles = new SortedDictionary<int, string>();
            foreach (var valor in Enum.GetValues(typeof(RolUsuario)))
            {
                roles.Add((int)valor, Enum.GetName(typeof(RolUsuario), valor) ?? "");
            }
            return roles;
        }
    }
}