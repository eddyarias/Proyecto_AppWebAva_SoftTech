using GaleriaArte.UsuarioService.Application.DTOs;

namespace GaleriaArte.UsuarioService.Application.Interfaces;

public interface IUsuarioService
{
    Task<object> RegistrarUsuarioAsync(UsuarioDto dto);
}
