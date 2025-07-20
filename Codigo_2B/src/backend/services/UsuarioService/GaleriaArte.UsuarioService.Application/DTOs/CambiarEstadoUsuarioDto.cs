namespace GaleriaArte.UsuarioService.Application.DTOs;
public class CambiarEstadoUsuarioDto
{
    public Guid UsuarioId { get; set; }
    public bool NuevoEstado { get; set; }
}