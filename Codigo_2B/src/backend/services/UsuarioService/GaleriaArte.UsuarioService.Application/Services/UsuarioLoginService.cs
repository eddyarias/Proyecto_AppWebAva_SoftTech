using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Data;
using BCrypt.Net;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using GaleriaArte.UsuarioService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
namespace GaleriaArte.UsuarioService.Application.Services;

public class UsuarioLoginService
{
    private readonly IUsuarioRepository _repo;
    private readonly IAuthService _authService;
    private readonly IAuditoriaPublisher _auditoriaPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioLoginService(IUsuarioRepository repo, IAuthService authService, IAuditoriaPublisher auditoriaPublisher,
            IHttpContextAccessor httpContextAccessor)
    {
        _repo = repo;
        _authService = authService;
        _auditoriaPublisher = auditoriaPublisher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        try
        {
            var usuario = await _repo.ObtenerPorNicknameOCorreoAsync(req.Identificador)
                      ?? throw new Exception("Usuario no encontrado");

            var clientIp = GetClientIpAddress();

            if (!BCrypt.Net.BCrypt.Verify(req.Contraseña, usuario.ContraseñaHash))
                throw new Exception("Credenciales inválidas");

            if (!usuario.Estado)
                throw new Exception("Usuario inactivo");

            (string tokenAcceso, string refreshToken) = await generarYGuardarTokens(usuario);

            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                    EventosUsuario.LOGIN_EXITOSO,
                    usuarioId: usuario.Id.ToString(),
                    rolId: usuario.RolId.ToString(),
                    ip: clientIp,
                    datos: new 
                    { 
                        nickname = usuario.Nickname
                    }
            );
            return new LoginResponse
            {
                TokenAcceso = tokenAcceso,
                RefreshToken = refreshToken
            };
        } catch (Exception ex)
        {
            // Publicar evento de auditoría de login fallido
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                EventosUsuario.LOGIN_FALLIDO,
                req.Identificador.ToString(),
                "Unknown",
                GetClientIpAddress(),
                new { Error = ex.Message }
            );
            throw;
        }
    }

    public async Task<(string tokenAcceso, string refreshToken)> generarYGuardarTokens(Usuario usuario)
    {
        var tokenAcceso = _authService.GenerarJwt(usuario);
        var refreshToken = _authService.GenerarRefreshToken();
        var exp = DateTime.UtcNow.AddDays(7);

        await _repo.ActualizarRefreshTokenAsync(usuario.Id, refreshToken, exp);
        return (tokenAcceso, refreshToken);
    }
    
    private string GetClientIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return "unknown";

        // Intentar obtener la IP real del cliente
        string ipAddress = context.Request.Headers["X-Forwarded-For"];
            
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = context.Request.Headers["X-Real-IP"];
                
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return ipAddress;
        }
    }