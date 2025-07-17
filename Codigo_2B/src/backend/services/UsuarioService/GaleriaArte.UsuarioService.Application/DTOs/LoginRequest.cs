namespace GaleriaArte.UsuarioService.Application.DTOs;

public class LoginRequest
{
    public string Identificador { get; set; } = null!; // nickname o correo
    public string Contraseña { get; set; } = null!;
}