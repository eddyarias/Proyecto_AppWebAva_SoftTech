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

            if (rol != EntityLayer.GaleriaArte.Util.Constants.ID_USUARIO_ARTISTA)
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
                var obra = (EntityLayer.GaleriaArte.Obra)e.Row.DataItem;

                Button btnEditar = (Button)e.Row.FindControl("btnEditar");
                Button btnOcultar = (Button)e.Row.FindControl("btnOcultar");
                Button btnEliminar = (Button)e.Row.FindControl("btnEliminar"); // ← lo agregamos abajo

                if (obra != null && btnOcultar != null)
                {
                    if (obra.estado == "activa")
                    {
                        btnOcultar.Text = "Ocultar";
                        btnOcultar.CssClass = "btn btn-danger btn-sm";
                        btnOcultar.OnClientClick = "return confirm('¿Estás seguro que deseas ocultar esta obra?');";
                        btnOcultar.CommandName = "OcultarObra";
                    }
                    else if (obra.estado == "vendida")
                    {
                        // Ocultar ambos botones
                        if (btnEditar != null) btnEditar.Visible = false;
                        if (btnOcultar != null) btnOcultar.Visible = false;
                    }
                    else if (obra.estado == "oculta")
                    {

                        btnOcultar.Text = "Mostrar";
                        btnOcultar.CssClass = "btn btn-success btn-sm";
                        btnOcultar.OnClientClick = "return confirm('¿Estás seguro que deseas mostrar esta obra?');";
                        btnOcultar.CommandName = "MostrarObra";

                        // Mostrar botón eliminar solo en estado oculta
                        if (btnEliminar != null)
                        {
                            btnEliminar.Visible = true;
                            btnEliminar.OnClientClick = "return confirm('¿Deseas eliminar esta obra?');";
                            btnEliminar.CommandName = "EliminarObra";
                            btnEliminar.CommandArgument = obra.id.ToString();
                        }
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
            else if (e.CommandName == "EliminarObra")
            {
                bool resultado = negocioObras.EliminarObra(obraId); 
                if (resultado)
                {
                    CargarObras();
                }
            }

        }

        protected void btnBuscarTitulo_Click(object sender, EventArgs e)
        {
            CargarObras(txtBuscarTitulo.Text.Trim());
        }

        protected void btnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            txtBuscarTitulo.Text = "";
            CargarObras();
        }

        private void CargarObras(string filtroTitulo = "")
        {
            if (Session["UsuarioID"] != null)
            {
                int artistaId = Convert.ToInt32(Session["UsuarioID"]);
                var negocioObras = new BusinessLayer.GaleriaArte.Obras(connectionString);

                var listaObras = negocioObras.ObtenerObrasPorArtista(artistaId);

                if (!string.IsNullOrWhiteSpace(filtroTitulo))
                {
                    listaObras = listaObras.FindAll(o => o.titulo.IndexOf(filtroTitulo, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                gvObras.DataSource = listaObras;
                gvObras.DataBind();
            }
            else
            {
                Response.Redirect("IniciarSesion.aspx");
            }
        }



    }
}