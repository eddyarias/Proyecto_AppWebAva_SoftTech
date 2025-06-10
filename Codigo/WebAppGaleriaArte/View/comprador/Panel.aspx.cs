using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.comprador
{
    public partial class Panel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verifica si hay un usuario en sesión
                var userSession = Session[EntityLayer.GaleriaArte.Util.Constants.NICKNAME] as EntityLayer.GaleriaArte.Usuarios;
                if (userSession != null)
                {
                    lblWelcome.Text = $"Bienvenido {userSession.nickname}";
                }
                else
                {
                    // Si no hay sesión activa, redirige al login
                    Response.Redirect("Login.aspx", true);
                }
            }
        }
    }
}