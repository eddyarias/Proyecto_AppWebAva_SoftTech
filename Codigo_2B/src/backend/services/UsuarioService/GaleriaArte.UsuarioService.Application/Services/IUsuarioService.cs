using GaleriaArte.UsuarioService.Application.DTOs;

namespace GaleriaArte.UsuarioService.Application.Services;

public interface IUsuarioService
{
    Task<object> RegistrarUsuarioAsync(UsuarioDto dto);
}
