using GaleriaArte.UsuarioService.Application.DTOs;

namespace GaleriaArte.UsuarioService.Application.Interfaces;

public interface IUsuarioService
{
    Task<bool> CambiarEstadoUsuarioAsync(Guid usuarioId, bool nuevoEstado);
    Task<object> RegistrarUsuarioAsync(UsuarioDto dto);
}
