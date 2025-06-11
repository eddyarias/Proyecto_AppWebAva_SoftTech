using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View
{
    public partial class Restablecer : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnRestablecer_Click(object sender, EventArgs e)
        {
            string tokenStr = Request.QueryString["token"];
            if (string.IsNullOrEmpty(tokenStr) || !Guid.TryParse(tokenStr, out Guid token))
            {
                lblMensaje.Text = "Token inválido.";
                return;
            }

            string nueva = txtNueva.Text;
            string confirmar = txtConfirmar.Text;

            if (nueva != confirmar)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            var bl = new BusinessLayer.GaleriaArte.RecuperacionPassword(connectionString);
            bool resultado = bl.RestablecerPassword(token, nueva);

            if (resultado)
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = "Contraseña actualizada correctamente.";
                Response.Redirect("/View/IniciarSesion.aspx");
            }
            else
            {
                lblMensaje.Text = "El token es inválido o ha expirado.";
            }
        }
    }
}