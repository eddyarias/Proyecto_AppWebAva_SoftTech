using System;
using System.Web.UI;
using EntityLayer.GaleriaArte;
using BusinessLayer.GaleriaArte;
using System.Configuration;

namespace WebAppGaleriaArte.View
{
    public partial class SingUp : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nickname = txtUsuario.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text;
            string rol = ddlRol.SelectedValue;

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(rol))
            {
                lblMensaje.Text = "Todos los campos son obligatorios.";
                return;
            }

            var usuarioBL = new BusinessLayer.GaleriaArte.Usuarios(connectionString);
            var existente = usuarioBL.GetByNickname(nickname);
            if (existente != null)
            {
                lblMensaje.Text = "El usuario ya existe.";
                return;
            }

            if (!EsCorreoMicrosoft(correo))
            {
                lblMensaje.Text = "Solo se permiten correos electrónicos de Microsoft (@outlook.com, @hotmail.com, @epn.edu.ec).";
                return;
            }

            var nuevoUsuario = new EntityLayer.GaleriaArte.Usuarios
            {
                nickname = nickname,
                correo = correo,
                contraseña_hash = password, 
                rol = rol,
                estado = true,
                fecha_creacion = DateTime.Now
            };

            int id = usuarioBL.Save(nuevoUsuario);
            if (id > 0)
            {
                lblMensaje.Text = "Registro exitoso. Ahora puedes iniciar sesión.";
            }
            else
            {
                lblMensaje.Text = "Error al registrar usuario.";
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("IniciarSesion.aspx");
        }

        private bool EsCorreoMicrosoft(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            correo = correo.ToLower();
            return correo.EndsWith("@outlook.com") || correo.EndsWith("@hotmail.com") || correo.EndsWith("@epn.edu.ec");
        }
    }
}