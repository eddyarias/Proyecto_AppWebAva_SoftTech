using DataAccess.GaleriaArte;
using EntityLayer.GaleriaArte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.GaleriaArte
{
    public class RecuperacionPassword
    {
        private string connectionString;

        public RecuperacionPassword(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool SolicitarRecuperacion(string correo)
        {
            var usuariosDA = new DataAccess.GaleriaArte.Usuarios(connectionString);
            var usuario = usuariosDA.GetByCorreo(correo);
            if (usuario == null) return false;

            Guid token = Guid.NewGuid();
            DateTime fecha_solicitud = DateTime.Now;
            DateTime expiracion = DateTime.Now.AddHours(1);

            var intento = new IntentoRecuperacion
            {
                usuario_id = usuario.id,
                token_recuperacion = token,
                expiracion = expiracion,
                fecha_solicitud = fecha_solicitud
            };

            var intentosDA = new IntentosRecuperacion(connectionString);
            intentosDA.GuardarIntento(intento);

            string url = $"http://localhost:53519/View/Restablecer.aspx?token={token}";
            string mensaje = $"Hola {usuario.nickname}, haz clic en el siguiente enlace para restablecer tu contraseña: {url}";

            return CorreoHelper.EnviarCorreo(correo, "Recuperar contraseña", mensaje);
        }

        public bool RestablecerPassword(Guid token, string nuevaPassword)
        {
            var intentosDA = new IntentosRecuperacion(connectionString);
            var intento = intentosDA.ObtenerPorToken(token);
            if (intento == null) return false;

            var usuariosDA = new DataAccess.GaleriaArte.Usuarios(connectionString);
            var nuevaPasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            usuariosDA.ActualizarPassword(intento.usuario_id, nuevaPasswordHash);
            intentosDA.MarcarComoUsado(intento.id);
            return true;
        }
    }
}
