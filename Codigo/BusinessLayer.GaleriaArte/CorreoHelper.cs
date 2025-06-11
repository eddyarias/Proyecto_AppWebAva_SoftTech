using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BusinessLayer.GaleriaArte
{
    public class CorreoHelper
    {
        public static bool EnviarCorreo(string destino, string asunto, string mensaje)
        {
            try
            {
                string emisor = ConfigurationManager.AppSettings["CorreoEmisor"];
                string clave = ConfigurationManager.AppSettings["CorreoClave"];

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emisor, clave)
                };

                var correo = new MailMessage(emisor, destino, asunto, mensaje);
                smtp.Send(correo);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
