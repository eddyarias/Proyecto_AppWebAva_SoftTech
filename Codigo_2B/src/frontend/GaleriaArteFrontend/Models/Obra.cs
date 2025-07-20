using System;

namespace GaleriaArteFrontend.Models
{
    public class Obra
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ArchivoBase64 { get; set; }
        public string ArtistaNickname { get; set; } // Cambiado de int a string para coincidir con el backend
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
