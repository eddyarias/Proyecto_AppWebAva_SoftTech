using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Data;
using BCrypt.Net;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using GaleriaArte.UsuarioService.Application.Interfaces;
namespace GaleriaArte.UsuarioService.Application.Services;

public class UsuarioLoginService
{
    private readonly IUsuarioRepository _repo;
    private readonly IAuthService _authService;

    public UsuarioLoginService(IUsuarioRepository repo, IAuthService authService)
    {
        _repo = repo;
        _authService = authService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var usuario = await _repo.ObtenerPorNicknameOCorreoAsync(req.Identificador)
                      ?? throw new Exception("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(req.Contraseña, usuario.ContraseñaHash))
            throw new Exception("Credenciales inválidas");

        var tokenAcceso = _authService.GenerarJwt(usuario);
        var refreshToken = _authService.GenerarRefreshToken();
        var exp = DateTime.UtcNow.AddDays(7);

        await _repo.ActualizarRefreshTokenAsync(usuario.Id, refreshToken, exp);

        return new LoginResponse
        {
            TokenAcceso = tokenAcceso,
            RefreshToken = refreshToken
        };
    }
}