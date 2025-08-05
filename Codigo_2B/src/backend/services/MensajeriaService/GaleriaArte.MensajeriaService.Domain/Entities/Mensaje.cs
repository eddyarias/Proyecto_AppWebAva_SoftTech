using System.ComponentModel.DataAnnotations;

namespace GaleriaArte.MensajeriaService.Domain.Entities
{
    public class Mensaje
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ConversacionId { get; set; }
        
        [Required]
        public string EmisorId { get; set; } = string.Empty;
        
        [Required]
        public string ReceptorId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Contenido { get; set; } = string.Empty;
        
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        
        public DateTime? FechaLeido { get; set; }
        
        public bool EsLeido { get; set; } = false;
        
        public bool EsEliminado { get; set; } = false;
        
        // Navegación
        public virtual Conversacion Conversacion { get; set; } = null!;
        
        // Métodos de ayuda
        public void MarcarComoLeido()
        {
            EsLeido = true;
            FechaLeido = DateTime.UtcNow;
        }
        
        public void Eliminar()
        {
            EsEliminado = true;
        }
    }
}
