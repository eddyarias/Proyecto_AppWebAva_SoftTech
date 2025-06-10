using System;
using System.Web.UI;

namespace WebAppGaleriaArte.View
{
    public partial class Recover : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            // Aquí iría la lógica para recuperar la cuenta
            lblMensaje.Text = "Si el correo existe, se enviarán instrucciones para recuperar la cuenta.";
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("IniciarSesion.aspx");

        }
    }
}