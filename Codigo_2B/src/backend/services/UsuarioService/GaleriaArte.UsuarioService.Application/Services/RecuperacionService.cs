using GaleriaArte.UsuarioService.Application.Interfaces;
using GaleriaArte.UsuarioService.Domain.Entities;
using GaleriaArte.UsuarioService.Domain.Interfaces;
using GaleriaArte.UsuarioService.Infrastructure.Repositories;

namespace GaleriaArte.UsuarioService.Application.Services;

public class RecuperacionService : IRecuperacionService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IRecuperacionRepository _recuperacionRepo;
    private readonly IEmailService _emailService;

    public RecuperacionService(IUsuarioRepository usuarioRepo, IRecuperacionRepository recuperacionRepo, IEmailService emailService)
    {
        _usuarioRepo = usuarioRepo;
        _recuperacionRepo = recuperacionRepo;
        _emailService = emailService;
    }

    public async Task<bool> SolicitarRecuperacionAsync(string correo)
    {
        var usuario = await _usuarioRepo.ObtenerPorCorreoAsync(correo);
        if (usuario == null) return false;

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

        return await _emailService.EnviarCorreoAsync(usuario.Correo, "Recuperar contraseña", mensaje);
    }

    public async Task<bool> RestablecerPasswordAsync(Guid token, string nuevaPassword)
    {
        var intento = await _recuperacionRepo.ObtenerPorTokenValidoAsync(token);
        if (intento == null) return false;

        var hash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
        await _usuarioRepo.ActualizarPasswordAsync(intento.UsuarioId, hash);
        await _recuperacionRepo.MarcarComoUsadoAsync(intento.Id);

        return true;
    }
}