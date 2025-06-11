using System;
using System.Collections.Generic;

namespace BusinessLayer.GaleriaArte
{
    public class Obras
    {
        private string strConnectionString;

        public Obras(string connectionString)
        {
            strConnectionString = connectionString;
        }

        // CREATE
        public int CrearObra(EntityLayer.GaleriaArte.Obra obra)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.CrearObra(obra);
        }

        // READ (por artista)
        public List<EntityLayer.GaleriaArte.Obra> ObtenerObrasPorArtista(int artistaId)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.ObtenerObrasPorArtista(artistaId);
        }

        // READ (por id)
        public EntityLayer.GaleriaArte.Obra ObtenerObraPorId(int id)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.ObtenerObraPorId(id);
        }

        // UPDATE
        public bool ActualizarObra(EntityLayer.GaleriaArte.Obra obra)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.ActualizarObra(obra);
        }

        // HIDE
        public bool OcultarObra(int obraId)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.OcultarObra(obraId);
        }

        // DELETE
        public bool EliminarObra(int obraId)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.EliminarObra(obraId);
        }

        // ACTIVE
        public bool ActivarObra(int obraId)
        {
            var dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.ActivarObra(obraId);
        }
    }
}
