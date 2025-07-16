namespace GaleriaArte.UsuarioService.Application.DTOs;

public class UsuarioDto
{
    public string Nickname { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Contraseña { get; set; } = null!;

    public string Rol { get; set; } = default!;
}