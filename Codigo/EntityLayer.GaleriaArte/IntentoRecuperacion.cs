using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.GaleriaArte
{
    public class IntentoRecuperacion
    {
        public int id { get; set; }
        public int usuario_id { get; set; }
        public Guid token_recuperacion { get; set; }
        public DateTime expiracion { get; set; }
        public bool usado { get; set; }
        public DateTime fecha_solicitud { get; set; }
    }
}
