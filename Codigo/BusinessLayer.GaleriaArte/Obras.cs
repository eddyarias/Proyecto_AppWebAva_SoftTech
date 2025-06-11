using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.GaleriaArte
{
    public class Obras
    {
        private string strConnectionString;
        public Obras(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public List<EntityLayer.GaleriaArte.Obra> ObtenerObrasPorArtista(int artistaId)
        {
            DataAccess.GaleriaArte.Obras dataAccess = new DataAccess.GaleriaArte.Obras(strConnectionString);
            return dataAccess.ObtenerObrasPorArtista(artistaId);
        }

    }


}
