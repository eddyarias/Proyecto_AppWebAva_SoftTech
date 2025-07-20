namespace GaleriaArte.UsuarioService.Domain.Entities;

public class IntentoRecuperacion
{
    public int Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid TokenRecuperacion { get; set; }
    public DateTime Expiracion { get; set; }
    public bool Usado { get; set; } = false;
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
}