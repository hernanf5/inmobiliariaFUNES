namespace inmobiliariaFUNES.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> BuscarPorPropietario(int idPropietario);

        IList<Inmueble> ObtenerLista(int paginaNro, int tamPagina, string? estado, int? idPropietario);
        int ObtenerCantidad(string? estado, int? idPropietario);
        int AgregarImagen(ImagenInmueble imagen);
        IList<Inmueble> ObtenerMasReservados(int dias);
        IList<Inmueble> ObtenerSinReservas(int dias);
        IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaDesde, DateTime fechaHasta);
        IList<ImagenInmueble> ObtenerImagenes(int idInmueble);
        int Reactivar(Inmueble entidad);
    }
}