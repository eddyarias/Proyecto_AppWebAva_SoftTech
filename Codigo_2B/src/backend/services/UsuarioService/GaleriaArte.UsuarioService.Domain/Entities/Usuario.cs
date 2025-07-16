namespace GaleriaArte.UsuarioService.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nickname { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string ContraseñaHash { get; set; } = null!;
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
}