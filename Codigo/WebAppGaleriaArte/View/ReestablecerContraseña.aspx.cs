using System;
using System.Configuration;
using System.Web.UI;

namespace WebAppGaleriaArte.View
{
    public partial class Recover : System.Web.UI.Page
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            var recBL = new BusinessLayer.GaleriaArte.RecuperacionPassword(connectionString);
            if (recBL.SolicitarRecuperacion(correo))
            {
                lblMensaje.Text = "Se enviaron las instrucciones a tu correo.";
            }
            else
            {
                lblMensaje.Text = "Correo no encontrado o error al enviar.";
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("IniciarSesion.aspx");

        }
    }
}