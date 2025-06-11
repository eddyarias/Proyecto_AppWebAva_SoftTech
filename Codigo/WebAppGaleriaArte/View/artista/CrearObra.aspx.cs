using System;
using System.Configuration;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLayer.GaleriaArte;
using EntityLayer.GaleriaArte;

namespace WebAppGaleriaArte.View.artista
{
    public partial class CrearObra : System.Web.UI.Page
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

            if (rol != "artista")
            {
                Response.Redirect("~/View/IniciarSesion.aspx");
            }

            lblMensaje.Text = "";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validación de sesión de artista
            if (Session["UsuarioID"] == null)
            {
                lblMensaje.Text = "Sesión expirada. Inicie sesión nuevamente.";
                return;
            }

            int artistaId = Convert.ToInt32(Session["UsuarioID"]);
            System.Diagnostics.Debug.WriteLine("artistaId: " + artistaId);


            // Validación de campos obligatorios
            if (!Page.IsValid)
            {
                lblMensaje.Text = "Complete los campos obligatorios.";
                return;
            }

            // Guardar imagen en el servidor
            string nombreArchivo = Guid.NewGuid().ToString(); // Nombre único
            string carpeta = ConfigurationManager.AppSettings["archivoFolderPath"]; // Ajusta la ruta si es necesario
            string resultadoImagen = "";

            if (fupArchivo.HasFile)
            {
                var fileManager = new WebAppGaleriaArte.View.FileManagement();
                resultadoImagen = fileManager.SaveImageOnServer(fupArchivo, carpeta, nombreArchivo);
                if (!string.IsNullOrEmpty(resultadoImagen))
                {
                    lblMensaje.Text = resultadoImagen;
                    return;
                }
            }
            else
            {
                lblMensaje.Text = "Debe seleccionar una imagen para la obra.";
                return;
            }

            // Construir la URL del archivo guardado
            string extension = System.IO.Path.GetExtension(fupArchivo.FileName).ToLower();
            string archivoUrl = carpeta + nombreArchivo + extension;


            // Parsear precio
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out precio))
            {
                lblMensaje.Text = "El precio no es válido.";
                return;
            }

            string rutaFisicaArchivo = Server.MapPath(archivoUrl);
            // Lee la clave privada desde el archivo XML
            string rutaClavePrivada = Server.MapPath("~/Recursos/Claves/privada.xml");
            string clavePrivadaXml = System.IO.File.ReadAllText(rutaClavePrivada);

            // Ahora puedes usar clavePrivadaXml normalmente
            string firmaDigital = BusinessLayer.GaleriaArte.DigitalSignatureService.FirmarArchivo(rutaFisicaArchivo, clavePrivadaXml);


            // Crear entidad Obra
            var obra = new Obra
            {
                titulo = txtTitulo.Text.Trim(),
                descripcion = txtDescripcion.Text.Trim(),
                archivo_url = archivoUrl,
                firma_digital = firmaDigital,
                artista_id = artistaId,
                precio = precio,
                estado = "activa",
            };

            // Guardar en base de datos
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;
            var negocioObras = new BusinessLayer.GaleriaArte.Obras(connectionString);

            try
            {
                int idObra = negocioObras.CrearObra(obra);
                if (idObra > 0)
                {
                    LimpiarFormulario();
                    Session["MensajeExito"] = "La obra fue subida exitosamente.";

                    // Redirige al panel de artista después de actualizar correctamente
                    Response.Redirect("~/View/artista/Panel.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    lblMensaje.Text = "No se pudo registrar la obra.";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar la obra: " + ex.Message;
            }
        }

        private void LimpiarFormulario()
        {
            txtTitulo.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
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
