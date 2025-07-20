using System.Net;
using System.Net.Mail;
using GaleriaArte.UsuarioService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GaleriaArte.UsuarioService.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> EnviarCorreoAsync(string destino, string asunto, string mensaje)
    {
        try
        {
            var emisor = _config["Correo:Emisor"];
            var clave = _config["Correo:Clave"];

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(emisor, clave)
            };

            var correo = new MailMessage(emisor, destino, asunto, mensaje);
            await smtp.SendMailAsync(correo);
            return true;
        }
        catch
        {
            return false;
        }
    }
}