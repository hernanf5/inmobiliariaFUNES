namespace inmobiliariaFUNES.Models
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
    {
        bool EstaOcupado(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int? idReservaExcluir = null);
        int Terminar(Reserva reserva, DateTime fechaTerminacion, decimal multa);
        IList<Reserva> ObtenerLista(int paginaNro, int tamPagina, string? estado);
        int ObtenerCantidad(string? estado);
        IList<Reserva> ObtenerQueTerminanEn(int dias);
    }
}