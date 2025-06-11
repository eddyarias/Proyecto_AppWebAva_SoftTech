using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.admin
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

            if (rol != "admin")
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
            }

            if (!IsPostBack)
            {                
                // Verifica si hay un usuario en sesión
                var userSession = Session[EntityLayer.GaleriaArte.Util.Constants.NICKNAME] as EntityLayer.GaleriaArte.Usuarios;
                if (userSession != null)
                {
                    lblWelcome.Text = $"Bienvenido {userSession.nickname}";
                }
            }
        }
    }
}