namespace GaleriaArte.UsuarioService.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nickname { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string ContraseñaHash { get; set; } = null!;
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExp { get; set; }
    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = default!;
}