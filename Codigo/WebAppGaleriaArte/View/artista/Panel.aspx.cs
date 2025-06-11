using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.artista
{
    public partial class Panel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtiene el nickname del usuario desde la sesión
                var nickname = Session["Usuario"] as string;
                if (!string.IsNullOrEmpty(nickname))
                {
                    lblWelcome.Text = $"Bienvenido {nickname}";
                }
                else
                {
                    // Si no hay sesión activa, redirige al login
                    //Response.Redirect("IniciarSesion.aspx", true);
                }
            }
        }
    }
}