using Microsoft.Extensions.Configuration;

namespace inmobiliariaFUNES.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            this.Configuration = configuration;
            connectionString = configuration.GetConnectionString("InmobiliariaConnection") ?? "";
        }
    }
}