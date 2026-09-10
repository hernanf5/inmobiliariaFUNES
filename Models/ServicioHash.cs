using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace inmobiliariaFUNES.Models
{
    public class ServicioHash
    {
        private readonly string salt;

        public ServicioHash(IConfiguration configuration)
        {
            salt = configuration["Salt"] ?? throw new InvalidOperationException("Falta configurar Salt en appsettings.json");
        }

        public string Hashear(string claveEnTextoPlano)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: claveEnTextoPlano,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
        }
        
    }
}