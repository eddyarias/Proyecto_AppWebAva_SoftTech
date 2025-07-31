

using GaleriaArte.UsuarioService.Application.DTOs;

namespace GaleriaArte.UsuarioService.Application.Interfaces
{
    public interface IAuditoriaPublisher
    {
        Task PublishEventoAsync(EventoAuditoriaDto evento);
        Task PublishUsuarioEventoAsync(string tipoEvento, string? usuarioId = null, string? rolId = null, string? ip = null, object? datos = null);
    }
}