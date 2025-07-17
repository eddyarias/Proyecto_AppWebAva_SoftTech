namespace GaleriaArte.UsuarioService.Domain.Entities;

public class UsuarioRol
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;

    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = default!;
}