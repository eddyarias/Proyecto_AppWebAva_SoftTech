using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.artista
{
    public partial class Panel : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtén el usuario desde la sesión
                var nickname = Session["Usuario"] as string;
                if (!string.IsNullOrEmpty(nickname))
                {
                    lblWelcome.Text = $"Bienvenido {nickname}";

                    // Obtener el ID del artista (puedes guardarlo en sesión al loguear)
                    int artistaId = Convert.ToInt32(Session["UsuarioID"]);

                    // Obtener las obras del artista
                    var negocioObras = new BusinessLayer.GaleriaArte.Obras(connectionString);
                    var listaObras = negocioObras.ObtenerObrasPorArtista(artistaId);

                    // Enlazar al GridView
                    gvObras.DataSource = listaObras;
                    gvObras.DataBind();
                }
                else
                {
                    // Si no hay sesión activa, redirige al login
                    //Response.Redirect("IniciarSesion.aspx", true);
                }
            }
        }
        protected void btnCrearObra_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearObra.aspx");
        }


    }
}