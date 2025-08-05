using System.ComponentModel.DataAnnotations;

namespace GaleriaArte.MensajeriaService.Domain.Entities
{
    public class Conversacion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Usuario1Id { get; set; } = string.Empty;
        
        [Required]
        public string Usuario2Id { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public DateTime UltimaActividad { get; set; } = DateTime.UtcNow;
        
        public bool EstaActivaUsuario1 { get; set; } = true;
        
        public bool EstaActivaUsuario2 { get; set; } = true;
        
        // Navegación
        public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
        
        // Métodos de ayuda
        public string ObtenerOtroUsuarioId(string usuarioActualId)
        {
            return usuarioActualId == Usuario1Id ? Usuario2Id : Usuario1Id;
        }
        
        public bool EsParticipante(string usuarioId)
        {
            return usuarioId == Usuario1Id || usuarioId == Usuario2Id;
        }
        
        public bool EstaActivaParaUsuario(string usuarioId)
        {
            return usuarioId == Usuario1Id ? EstaActivaUsuario1 : EstaActivaUsuario2;
        }
        
        public void ActualizarActividad()
        {
            UltimaActividad = DateTime.UtcNow;
        }
    }
}
