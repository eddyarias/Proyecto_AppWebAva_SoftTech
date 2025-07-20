using System;

namespace GaleriaArteFrontend.Models
{
    public class Obra
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public int ArtistaId { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
