using Microsoft.AspNetCore.Http;

namespace GaleriaArte.ObraService.Application.DTOs;

public class CreateObraDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public IFormFile Archivo { get; set; } = null!; // Archivo JPG a convertir en base64
    public string ArtistaNickname { get; set; } = null!;
    public decimal Precio { get; set; }
}