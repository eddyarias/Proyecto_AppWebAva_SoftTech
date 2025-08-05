namespace GaleriaArte.MensajeriaService.Domain.DTOs
{
    public class MensajeDto
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public string EmisorId { get; set; } = string.Empty;
        public string EmisorNickname { get; set; } = string.Empty;
        public string ReceptorId { get; set; } = string.Empty;
        public string ReceptorNickname { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaLeido { get; set; }
        public bool EsLeido { get; set; }
        public bool EsMio { get; set; }
    }

    public class EnviarMensajeDto
    {
        public string ReceptorId { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public int? ConversacionId { get; set; }
    }

    public class ConversacionDto
    {
        public int Id { get; set; }
        public string OtroUsuarioId { get; set; } = string.Empty;
        public string OtroUsuarioNickname { get; set; } = string.Empty;
        public DateTime UltimaActividad { get; set; }
        public MensajeDto? UltimoMensaje { get; set; }
        public int MensajesNoLeidos { get; set; }
        public bool EstaConectado { get; set; }
        public DateTime? UltimaConexion { get; set; }
        public bool EstaEscribiendo { get; set; }
    }

    public class EstadoUsuarioDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public bool EstaConectado { get; set; }
        public DateTime? UltimaConexion { get; set; }
        public bool EstaEscribiendo { get; set; }
        public string? EscribiendoEnConversacion { get; set; }
    }

    public class NotificacionMensajeDto
    {
        public int ConversacionId { get; set; }
        public MensajeDto Mensaje { get; set; } = null!;
        public ConversacionDto Conversacion { get; set; } = null!;
    }
}
