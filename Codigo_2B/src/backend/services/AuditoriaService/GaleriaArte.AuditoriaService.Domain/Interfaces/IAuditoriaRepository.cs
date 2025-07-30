using GaleriaArte.AuditoriaService.Domain.Entities;

namespace GaleriaArte.AuditoriaService.Domain.Interfaces
{
    public interface IAuditoriaRepository
    {
        Task<int> CrearLogEventoAsync(LogEvento logEvento);
        Task<IEnumerable<LogEvento>> ObtenerLogsPorMicroservicioAsync(string microservicio, int limite = 100);
        Task<IEnumerable<LogEvento>> ObtenerLogsPorUsuarioAsync(string usuarioId, int limite = 100);
        Task<IEnumerable<LogEvento>> ObtenerLogsPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}