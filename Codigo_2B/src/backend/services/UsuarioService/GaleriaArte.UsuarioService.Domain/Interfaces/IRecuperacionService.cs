
namespace GaleriaArte.UsuarioService.Domain.Interfaces;

public interface IRecuperacionService
{
    Task<bool> SolicitarRecuperacionAsync(string correo);
    Task<bool> RestablecerPasswordAsync(Guid token, string nuevaPassword);
}