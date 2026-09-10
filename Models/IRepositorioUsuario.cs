namespace inmobiliariaFUNES.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
    }
}