namespace GaleriaArte.MensajeriaService.Application.Services
{
    public interface IConexionService
    {
        Task RegistrarConexionAsync(string usuarioId, string connectionId);
        Task RegistrarDesconexionAsync(string connectionId);
        Task<bool> EstaConectadoAsync(string usuarioId);
        Task<List<string>> ObtenerConexionesAsync(string usuarioId);
        Task IniciarEscrituraAsync(string usuarioId, string conversacionId);
        Task DetenerEscrituraAsync(string usuarioId);
        Task<bool> EstaEscribiendoAsync(string usuarioId, string conversacionId);
        Task LimpiarConexionesAsync();
    }
}
