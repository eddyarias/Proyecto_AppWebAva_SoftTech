namespace GaleriaArte.UsuarioService.Application.DTOs;

public class LoginResponse
{
    public string TokenAcceso { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}