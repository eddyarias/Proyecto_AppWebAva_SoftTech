using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.artista
{
    public partial class Panel : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
                return;
            }

            FormsIdentity identity = (FormsIdentity)User.Identity;
            string rol = identity.Ticket.UserData;

            if (rol != "artista")
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
            }

            if (!IsPostBack)
            {
                var nickname = Session["Usuario"] as string;
                if (!string.IsNullOrEmpty(nickname))
                {
                    lblWelcome.Text = $"Bienvenido {nickname}";
                    if (Session["MensajeExito"] != null)
                    {
                        lblMensajeExito.Text = Session["MensajeExito"].ToString();
                        Session.Remove("MensajeExito");

                        // Registrar script para ocultar el label después de 5 segundos
                        string script = @"
                setTimeout(function() {
                    var lbl = document.getElementById('" + lblMensajeExito.ClientID + @"');
                    if(lbl) { lbl.style.display = 'none'; }
                }, 5000);"; // 5000 ms = 5 segundos

                        ClientScript.RegisterStartupScript(this.GetType(), "HideLabel", script, true);
                    }

                    CargarObras();
                }
                else
                {
                    Response.Redirect("IniciarSesion.aspx");
                }
            }
        }

        protected void btnCrearObra_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearObra.aspx");
        }

        protected void gvObras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtén el objeto vinculado a esta fila
                var obra = (EntityLayer.GaleriaArte.Obra)e.Row.DataItem;

                // Encuentra el botón "btnOcultar"
                Button btnOcultar = (Button)e.Row.FindControl("btnOcultar");

                if (obra != null && btnOcultar != null)
                {
                    if (obra.estado == "activa")
                    {
                        btnOcultar.Text = "Ocultar";
                        btnOcultar.CssClass = "btn btn-danger btn-sm";
                        btnOcultar.OnClientClick = "return confirm('¿Estás seguro que deseas ocultar esta obra?');";
                        btnOcultar.CommandName = "OcultarObra";
                    }
                    else if (obra.estado == "oculta")
                    {
                        btnOcultar.Text = "Mostrar";
                        btnOcultar.CssClass = "btn btn-success btn-sm";
                        btnOcultar.OnClientClick = "return confirm('¿Estás seguro que deseas mostrar esta obra?');";
                        btnOcultar.CommandName = "MostrarObra";
                    }
                }
            }
        }

        protected void gvObras_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int obraId = Convert.ToInt32(e.CommandArgument);
            var negocioObras = new BusinessLayer.GaleriaArte.Obras(connectionString);

            if (e.CommandName == "EditarObra")
            {
                Response.Redirect($"EditarObra.aspx?id={obraId}");
            }
            else if (e.CommandName == "OcultarObra")
            {
                bool resultado = negocioObras.OcultarObra(obraId);
                if (resultado)
                {
                    CargarObras();
                }
            }
            else if (e.CommandName == "MostrarObra")
            {
                bool resultado = negocioObras.ActivarObra(obraId); // Método que debes implementar
                if (resultado)
                {
                    CargarObras();
                }
            }
        }



        private void CargarObras()
        {
            // Obtener el ID del artista desde la sesión
            if (Session["UsuarioID"] != null)
            {
                int artistaId = Convert.ToInt32(Session["UsuarioID"]);

                // Crear instancia del negocio con la cadena de conexión
                var negocioObras = new BusinessLayer.GaleriaArte.Obras(connectionString);

                // Obtener las obras del artista
                var listaObras = negocioObras.ObtenerObrasPorArtista(artistaId);

                // Asignar la fuente de datos y enlazar al GridView
                gvObras.DataSource = listaObras;
                gvObras.DataBind();
            }
            else
            {
                // Opcional: manejar si no hay sesión activa, por ejemplo redirigir a login
                Response.Redirect("IniciarSesion.aspx");
            }
        }
    }
}