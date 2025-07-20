using System.Threading.Tasks;
using GaleriaArteFrontend.Models;
using System.Collections.Generic;

namespace GaleriaArteFrontend.Services
{
    public class ObrasService
    {
        // Aquí se implementarán los métodos para consumir el API Gateway
        public Task<List<Obra>> GetObrasAsync()
        {
            // Lógica para obtener obras desde el backend
            return Task.FromResult(new List<Obra>());
        }
    }
}
