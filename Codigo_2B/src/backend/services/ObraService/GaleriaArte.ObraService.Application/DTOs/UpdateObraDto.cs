namespace GaleriaArte.ObraService.Application.DTOs;

public class UpdateObraDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
}
