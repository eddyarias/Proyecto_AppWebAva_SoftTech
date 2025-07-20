namespace GaleriaArte.UsuarioService.Application.DTOs;

public class RestablecerPasswordDto
{
    public Guid Token { get; set; }
    public string NuevaPassword { get; set; } = string.Empty;
}