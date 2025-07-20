namespace GaleriaArte.UsuarioService.Application.Interfaces;

public interface IEmailService
{
    Task<bool> EnviarCorreoAsync(string destino, string asunto, string mensaje);
}