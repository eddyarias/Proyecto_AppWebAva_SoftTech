namespace GaleriaArte.ObraService.Application.DTOs;

public class CreateObraDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string ArchivoUrl { get; set; } = null!;
    public string FirmaDigital { get; set; } = null!;
    public string ArtistaNickname { get; set; } = null!;
    public decimal Precio { get; set; }
}