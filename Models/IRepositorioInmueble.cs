namespace inmobiliariaFUNES.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> BuscarPorPropietario(int idPropietario);
        int AgregarImagen(ImagenInmueble imagen);
        IList<ImagenInmueble> ObtenerImagenes(int idInmueble);
        int Reactivar(Inmueble entidad);
    }
}