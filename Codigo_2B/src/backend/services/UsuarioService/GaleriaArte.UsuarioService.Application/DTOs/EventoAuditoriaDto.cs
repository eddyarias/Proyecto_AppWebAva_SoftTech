using System.Text.Json;

namespace GaleriaArte.UsuarioService.Application.DTOs
{
    public class EventoAuditoriaDto
    {
        public string Microservicio { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
        public string? UsuarioId { get; set; }
        public string? RolId { get; set; }
        public string? Ip { get; set; }
        public object? Datos { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string DatosJson => Datos != null ? JsonSerializer.Serialize(Datos) : string.Empty;
    }

    // Eventos específicos del UsuarioService
    public class UsuarioEventoAuditoria : EventoAuditoriaDto
    {
        public UsuarioEventoAuditoria()
        {
            Microservicio = "UsuarioService";
        }
    }

    public static class EventosUsuario
    {
        public const string LOGIN_EXITOSO = "LOGIN_EXITOSO";
        public const string LOGIN_FALLIDO = "LOGIN_FALLIDO";
        public const string REGISTRO_USUARIO = "REGISTRO_USUARIO";
        public const string CAMBIO_PASSWORD = "CAMBIO_PASSWORD";
        public const string RECUPERACION_PASSWORD = "RECUPERACION_PASSWORD";
        public const string CAMBIO_ESTADO_USUARIO = "CAMBIO_ESTADO_USUARIO";
        public const string REFRESH_TOKEN = "REFRESH_TOKEN";
        public const string LOGOUT = "LOGOUT";
    }
}