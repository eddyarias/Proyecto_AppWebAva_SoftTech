using GaleriaArte.UsuarioService.Domain.Entities;

namespace GaleriaArte.UsuarioService.Infrastructure.Repositories;

public interface IRecuperacionRepository
{
    Task GuardarIntentoAsync(IntentoRecuperacion intento);
    Task<IntentoRecuperacion?> ObtenerPorTokenValidoAsync(Guid token);
    Task MarcarComoUsadoAsync(int id);
}