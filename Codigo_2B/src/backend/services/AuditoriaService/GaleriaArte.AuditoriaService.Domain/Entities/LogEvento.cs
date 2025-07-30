namespace GaleriaArte.AuditoriaService.Domain.Entities

{
    public class LogEvento
    {
        public int Id { get; set; }
        public string Microservicio { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
        public string? UsuarioId { get; set; }
        public string? RolId { get; set; }
        public string? Ip { get; set; }
        public string? Datos { get; set; } // JSON serializado
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}