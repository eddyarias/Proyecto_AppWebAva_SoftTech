using System;

namespace EntityLayer.GaleriaArte
{
    [Serializable]
    public class Obra
    {
        public Obra() { }


        public Obra(int id, string titulo, string descripcion, string archivo_url, string firma_digital, int artista_id, decimal precio, string estado, DateTime fecha_publicacion)
        {
            this.id = id;
            this.titulo = titulo;
            this.descripcion = descripcion;
            this.archivo_url = archivo_url;
            this.firma_digital = firma_digital;
            this.artista_id = artista_id;
            this.precio = precio;
            this.estado = estado;
            this.fecha_publicacion = fecha_publicacion;
        }

        public int id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string archivo_url { get; set; }
        public string firma_digital { get; set; }
        public int artista_id { get; set; }
        public decimal precio { get; set; }
        public string estado { get; set; }
        public DateTime fecha_publicacion { get; set; }
    }
}
