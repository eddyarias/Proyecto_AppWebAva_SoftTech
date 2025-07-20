namespace GaleriaArte.ObraService.Application.DTOs;

public class ObraDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string ArchivoBase64 { get; set; } = string.Empty; // Imagen JPG en formato base64
    public string FirmaDigital { get; set; } = string.Empty;
    public string ArtistaNickname { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
}
