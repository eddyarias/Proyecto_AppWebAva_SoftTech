using GaleriaArte.UsuarioService.Application.DTOs;
using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace GaleriaArte.UsuarioService.Application.Services;

public class RecuperacionService : IRecuperacionService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IRecuperacionRepository _recuperacionRepo;
    private readonly IEmailService _emailService;
    private readonly IAuditoriaPublisher _auditoriaPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecuperacionService(IUsuarioRepository usuarioRepo, IRecuperacionRepository recuperacionRepo, IEmailService emailService, IAuditoriaPublisher auditoriaPublisher,
            IHttpContextAccessor httpContextAccessor)
    {
        _usuarioRepo = usuarioRepo;
        _recuperacionRepo = recuperacionRepo;
        _emailService = emailService;
        _auditoriaPublisher = auditoriaPublisher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> SolicitarRecuperacionAsync(string correo)
    {
        try
        {
            var usuario = await _usuarioRepo.ObtenerPorCorreoAsync(correo);
            if (usuario == null) throw new Exception("Usuario no encontrado");

            var token = Guid.NewGuid();
            var intento = new IntentoRecuperacion
            {
                UsuarioId = usuario.Id,
                TokenRecuperacion = token,
                Expiracion = DateTime.UtcNow.AddHours(1)
            };

            await _recuperacionRepo.GuardarIntentoAsync(intento);

            var url = $"http://localhost:5001/restablecer-password/{token}";
            var mensaje = $"Hola {usuario.Nickname}, haz clic en el siguiente enlace para restablecer tu contraseña: {url}";

            // Publicar solicitud exitosa
            var clientIp = GetClientIpAddress();
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                    EventosUsuario.RECUPERACION_PASSWORD,
                    usuarioId: usuario.Id.ToString(),
                    rolId: usuario.RolId.ToString(),
                    ip: clientIp,
                    datos: new
                    {
                        nickname = usuario.Nickname,
                        correo = usuario.Correo
                    }
            );
            return await _emailService.EnviarCorreoAsync(usuario.Correo, "Recuperar contraseña", mensaje);
        }
        catch (Exception ex)
        {
            // Publicar evento de recuperacion fallido
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                EventosUsuario.RECUPERACION_PASSWORD,
                correo,
                "unknown",
                GetClientIpAddress(),
                new { Error = ex.Message }
            );
            return false;
        }
    }

    public async Task<bool> RestablecerPasswordAsync(Guid token, string nuevaPassword)
    {
        try
        {
            var intento = await _recuperacionRepo.ObtenerPorTokenValidoAsync(token);
            if (intento == null) 
                throw new Exception("Intento de recuperación no válido o expirado");

            var hash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            await _usuarioRepo.ActualizarPasswordAsync(intento.UsuarioId, hash);
            await _recuperacionRepo.MarcarComoUsadoAsync(intento.Id);

            // Publicar cambio password exitoso
            var clientIp = GetClientIpAddress();
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                    EventosUsuario.REGISTRO_USUARIO,
                    usuarioId: intento.UsuarioId.ToString(),
                    rolId: "unknown",
                    ip: clientIp,
                    datos: new
                    {
                        fecha_solicitud = intento.FechaSolicitud,
                        usado = intento.Usado
                    }
            );
            return true;
        }catch (Exception ex)
        {
            // Publicar evento reestablecer fallido
            await _auditoriaPublisher.PublishUsuarioEventoAsync(
                EventosUsuario.RECUPERACION_PASSWORD,
                "unknown",
                "unknown",
                GetClientIpAddress(),
                new { Error = ex.Message }
            );
            return false;
        }
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