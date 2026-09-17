using System.Collections.Generic;

namespace inmobiliariaFUNES.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        IList<Inquilino> BuscarPorNombre(string nombre);
    }
}