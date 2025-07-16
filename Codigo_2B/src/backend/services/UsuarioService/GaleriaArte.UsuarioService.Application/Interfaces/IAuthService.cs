using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Entities;

namespace GaleriaArte.UsuarioService.Application.Interfaces;

public interface IAuthService
{
    string GenerarJwt(Usuario usuario);
    string GenerarRefreshToken();
}