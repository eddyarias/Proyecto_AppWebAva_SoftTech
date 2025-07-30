using Microsoft.EntityFrameworkCore;
using GaleriaArte.AuditoriaService.Domain.Entities;
using GaleriaArte.AuditoriaService.Domain.Interfaces;
using GaleriaArte.AuditoriaService.Infrastructure.Data;

namespace GaleriaArte.AuditoriaService.Infrastructure.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AuditoriaDbContext _context;

        public AuditoriaRepository(AuditoriaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearLogEventoAsync(LogEvento logEvento)
        {
            _context.LogsEventos.Add(logEvento);
            await _context.SaveChangesAsync();
            return logEvento.Id;
        }

        public async Task<IEnumerable<LogEvento>> ObtenerLogsPorMicroservicioAsync(string microservicio, int limite = 100)
        {
            return await _context.LogsEventos
                .Where(l => l.Microservicio == microservicio)
                .OrderByDescending(l => l.Fecha)
                .Take(limite)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEvento>> ObtenerLogsPorUsuarioAsync(string usuarioId, int limite = 100)
        {
            return await _context.LogsEventos
                .Where(l => l.UsuarioId == usuarioId)
                .OrderByDescending(l => l.Fecha)
                .Take(limite)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEvento>> ObtenerLogsPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.LogsEventos
                .Where(l => l.Fecha >= fechaInicio && l.Fecha <= fechaFin)
                .OrderByDescending(l => l.Fecha)
                .ToListAsync();
        }
    }
}
