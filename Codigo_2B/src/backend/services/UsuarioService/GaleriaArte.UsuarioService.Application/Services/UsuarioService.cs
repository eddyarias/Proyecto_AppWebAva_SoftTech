using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Infrastructure.Data;
using BCrypt.Net;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using GaleriaArte.UsuarioService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GaleriaArte.UsuarioService.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _repositorio;
    private readonly UsuarioLoginService _loginService;
    private readonly IAuditoriaPublisher _auditoriaPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioService(UsuarioRepository repository, UsuarioLoginService loginService, IAuditoriaPublisher auditoriaPublisher,
            IHttpContextAccessor httpContextAccessor)
    {
        _repositorio = repository;
        _loginService = loginService;
        _auditoriaPublisher = auditoriaPublisher;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<object> RegistrarUsuarioAsync(UsuarioDto dto)
    {
        try
        {
            if (await _repositorio.ExisteCorreoAsync(dto.Correo))
                throw new Exception("Correo ya registrado.");

            if (await _repositorio.ExisteNicknameAsync(dto.Nickname))
                throw new Exception("Nickname ya registrado.");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contraseña);
            var usuario = new Usuario
            {
                Nickname = dto.Nickname,
                Correo = dto.Correo,
                ContraseñaHash = hash
            };

            var rol = await _repositorio.ObtenerRolPorNombreAsync(dto.Rol.ToLower());
            if (rol == null)
                throw new Exception("Rol inválido.");

            usuario.Rol = rol;
            usuario.RolId = rol.Id;

            await _repositorio.AgregarUsuarioAsync(usuario);
            // Publicar registro exitoso
            var clientIp = GetClientIpAddress();
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                    EventosUsuario.REGISTRO_USUARIO,
                    usuarioId: usuario.Id.ToString(),
                    rolId: usuario.RolId.ToString(),
                    ip: clientIp,
                    datos: new
                    {
                        nickname = usuario.Nickname
                    }
            );
            // Devolvemos un objeto anónimo con los datos necesarios
            return new
            {
                success = true,
                message = "Usuario registrado exitosamente.",
            };
        }catch (Exception ex)
        {
            // Publicar evento de registro fallido
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                EventosUsuario.REGISTRO_USUARIO,
                dto.Correo.ToString(),
                dto.Rol.ToString(),
                GetClientIpAddress(),
                new { Error = ex.Message }
            );
            throw;
        }
    }
    
    public async Task<bool> CambiarEstadoUsuarioAsync(Guid usuarioId, bool nuevoEstado)
    {
        try
        {
            var usuario = await _repositorio.ObtenerPorIdAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            try
            {
                usuario.Estado = nuevoEstado;
                await _repositorio.ActualizarUsuarioAsync(usuario);
                // Publicar CAMBIO DE ETADO exitoso
                var clientIp = GetClientIpAddress();
                await _auditoriaPublisher.PublishUsuarioEventoAsync(
                        EventosUsuario.CAMBIO_ESTADO_USUARIO,
                        usuarioId: "administrador",
                        rolId: "f50fdbe5-2e16-4e91-9e7b-a39219d57031",
                        ip: clientIp,
                        datos: new
                        {
                            usuarioId = usuario.Id.ToString(),
                            nuevoEstado = usuario.Estado,
                            usuarioNickname = usuario.Nickname
                        }
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar el estado del usuario.");
            }
        }catch (Exception ex)
        {
            // Publicar evento de auditoría de cambio de estado fallido
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                EventosUsuario.CAMBIO_ESTADO_USUARIO,
                "administrador",
                "f50fdbe5-2e16-4e91-9e7b-a39219d57031",
                GetClientIpAddress(),
                new { Error = ex.Message }
            );
            return false;
        }
    }

    public async Task<TokenResponseDto?> RenovarTokenAsync(string refreshToken)
    {
        var usuario = await _repositorio.ObtenerPorRefreshTokenAsync(refreshToken);

        if (usuario == null)
            return null; // Token inexistente

        var expirationDate = usuario.RefreshTokenExp;
        if (expirationDate < DateTime.UtcNow)
            return null; // Token expirado

        // Genera nuevos tokens
        (string tokenAcceso, string newRefreshToken) = await _loginService.generarYGuardarTokens(usuario);

        return new TokenResponseDto
        {
            AccessToken = tokenAcceso,
            RefreshToken = newRefreshToken
        };
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