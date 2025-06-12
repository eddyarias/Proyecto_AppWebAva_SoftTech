using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.comprador
{
    public partial class Panel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
                return;
            }

            FormsIdentity identity = (FormsIdentity)User.Identity;
            string rol = identity.Ticket.UserData;

            if (rol != EntityLayer.GaleriaArte.Util.Constants.ID_USUARIO_COMPRADOR)
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
            }

            if (!IsPostBack)
            {
                // Obtiene el nickname del usuario desde la sesión
                var nickname = Session["Usuario"] as string;
                if (!string.IsNullOrEmpty(nickname))
                {
                    lblWelcome.Text = $"Bienvenido {nickname}";
                }
                CargarObras();
            }
        }

        private void CargarObras()
        {
            string connStr = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
            var obrasBL = new BusinessLayer.GaleriaArte.Obras(connStr);
            var obrasActivas = obrasBL.ObtenerObrasActivas(10);

            rptObras.DataSource = obrasActivas;
            rptObras.DataBind();
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Limpiar la sesión
            Session.Clear();
            Session.Abandon();

            // Cerrar la autenticación Forms
            FormsAuthentication.SignOut();

            // Redirigir a la página de inicio de sesión
            Response.Redirect("~/View/IniciarSesion.aspx");
        }
    }
}