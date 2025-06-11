using EntityLayer.GaleriaArte;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppGaleriaArte.View.artista
{
    public partial class EditarObra : System.Web.UI.Page
    {
        private int obraId;


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

            lblMensaje.Text = "";

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out obraId))
                {
                    lblMensaje.Text = "ID de obra no válido.";
                    return;
                }

                obraId = int.Parse(Request.QueryString["id"]);
                CargarObra(obraId);
            }
        }

        private void CargarObra(int id)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
            var negocio = new BusinessLayer.GaleriaArte.Obras(connectionString);
            var obra = negocio.ObtenerObraPorId(id); // Asegúrate de tener este método o implementarlo

            if (obra != null)
            {
                txtTitulo.Text = obra.titulo;
                txtDescripcion.Text = obra.descripcion;
                txtPrecio.Text = obra.precio.ToString(CultureInfo.InvariantCulture);
                ViewState["Obra"] = obra;
            }
            else
            {
                lblMensaje.Text = "Obra no encontrada.";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!(ViewState["Obra"] is Obra obra))
            {
                lblMensaje.Text = "Error al cargar datos de la obra.";
                return;
            }

            obra.titulo = txtTitulo.Text.Trim();
            obra.descripcion = txtDescripcion.Text.Trim();

            if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio))
            {
                lblMensaje.Text = "El precio es inválido.";
                return;
            }

            obra.precio = precio;

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
            var negocio = new BusinessLayer.GaleriaArte.Obras(connectionString);

            try
            {
                bool actualizado = negocio.ActualizarObra(obra);
                if (actualizado)
                {
                    Session["MensajeExito"] = "La obra fue actualizada exitosamente.";

                    // Redirige al panel de artista después de actualizar correctamente
                    Response.Redirect("~/View/artista/Panel.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    lblMensaje.Text = "No se pudo actualizar la obra.";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // Redirige al panel de artista después de actualizar correctamente
            Response.Redirect("~/View/artista/Panel.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }
}