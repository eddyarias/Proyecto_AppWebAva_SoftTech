namespace GaleriaArte.ObraService.Domain.Entities;

public class Obra
{
    public int Id { get; set; } // SERIAL en la base de datos
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string ArchivoUrl { get; set; } = string.Empty;
    public string FirmaDigital { get; set; } = string.Empty;
    public string ArtistaNickname { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Estado { get; set; } = Estados.Activa;
    public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

    public static class Estados
    {
        public const string Activa = "activa";
        public const string Vendida = "vendida";
        public const string Oculta = "oculta";
    }
}