using GaleriaArte.MensajeriaService.Domain.DTOs;

namespace GaleriaArte.MensajeriaService.Application.Services
{
    public interface IMensajeriaService
    {
        Task<MensajeDto?> EnviarMensajeAsync(string emisorId, EnviarMensajeDto mensaje);
        Task<List<ConversacionDto>> ObtenerConversacionesAsync(string usuarioId);
        Task<List<MensajeDto>> ObtenerMensajesAsync(int conversacionId, string usuarioId, int pagina = 1, int tamanoPagina = 50);
        Task<ConversacionDto?> ObtenerConversacionAsync(int conversacionId, string usuarioId);
        Task<ConversacionDto?> CrearOObtenerConversacionAsync(string usuario1Id, string usuario2Id);
        Task MarcarMensajesComoLeidosAsync(int conversacionId, string usuarioId);
        Task<List<string>> ObtenerContactosAsync(string usuarioId);
        Task<int> ContarMensajesNoLeidosAsync(string usuarioId);
        Task<bool> ExisteConversacionAsync(string usuario1Id, string usuario2Id);
    }
}
