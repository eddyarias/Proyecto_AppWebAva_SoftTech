using System.Threading.Tasks;
using GaleriaArteFrontend.Models;

namespace GaleriaArteFrontend.Services
{
    public class AuthService
    {
        // Métodos para autenticación y autorización
        public Task<Usuario> LoginAsync(string email, string password)
        {
            // Lógica para autenticación contra el API Gateway
            return Task.FromResult<Usuario>(null);
        }
    }
}
