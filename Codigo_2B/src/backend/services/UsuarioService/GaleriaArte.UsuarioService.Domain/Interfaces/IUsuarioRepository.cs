using GaleriaArte.UsuarioService.Domain.Entities;

namespace GaleriaArte.UsuarioService.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<bool> ExisteCorreoAsync(string correo);
    Task<bool> ExisteNicknameAsync(string nickname);
    Task<Rol?> ObtenerRolPorNombreAsync(string nombre);
    Task AgregarUsuarioAsync(Usuario usuario);
    Task<Usuario?> ObtenerPorNicknameOCorreoAsync(string identificador);
    Task ActualizarRefreshTokenAsync(Guid id, string refreshToken, DateTime expira);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task ActualizarPasswordAsync(Guid usuarioId, string hash);
}