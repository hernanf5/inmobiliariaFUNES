using System.Collections.Generic;

namespace inmobiliariaFUNES.Models
{
    public interface IRepositorio<T>
    {
        int Alta(T p);
        int Baja(T p);
        int Modificacion(T p);
        IList<T> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        T? ObtenerPorId(int id);
    }
}