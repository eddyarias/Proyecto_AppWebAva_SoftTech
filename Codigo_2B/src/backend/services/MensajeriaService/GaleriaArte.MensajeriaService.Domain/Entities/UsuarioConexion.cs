using System.ComponentModel.DataAnnotations;

namespace GaleriaArte.MensajeriaService.Domain.Entities
{
    public class UsuarioConexion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        
        [Required]
        public string ConnectionId { get; set; } = string.Empty;
        
        public DateTime FechaConexion { get; set; } = DateTime.UtcNow;
        
        public DateTime UltimaActividad { get; set; } = DateTime.UtcNow;
        
        public bool EstaConectado { get; set; } = true;
        
        public bool EstaEscribiendo { get; set; } = false;
        
        public string? EscribiendoEnConversacion { get; set; }
        
        // Métodos de ayuda
        public void ActualizarActividad()
        {
            UltimaActividad = DateTime.UtcNow;
        }
        
        public void IniciarEscritura(string conversacionId)
        {
            EstaEscribiendo = true;
            EscribiendoEnConversacion = conversacionId;
            ActualizarActividad();
        }
        
        public void DetenerEscritura()
        {
            EstaEscribiendo = false;
            EscribiendoEnConversacion = null;
            ActualizarActividad();
        }
        
        public void Desconectar()
        {
            EstaConectado = false;
            EstaEscribiendo = false;
            EscribiendoEnConversacion = null;
            ActualizarActividad();
        }
    }
}
