namespace GaleriaArte.UsuarioService.Domain.Entities;

public class Rol
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    
}