using System;
using System.Collections.Generic;
using BusinessLayer.GaleriaArte;
using EntityLayer.GaleriaArte;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace WebAppGaleriaArte.View
{
    public partial class Login : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GaleriaArte"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtUsuario.Text = "esteban";
                txtPassword.Text = "123456789";
                lblMensaje.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            string usuario = txtUsuario.Text.Trim();
            string contraseña = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                lblMensaje.Text = "Debe llenar los campos de usuario y contraseña.";
                return;
            }

            var negocioUsuarios = new BusinessLayer.GaleriaArte.Usuarios(connectionString);
            EntityLayer.GaleriaArte.Usuarios usuarioAutenticado = negocioUsuarios.Authenticate(usuario, contraseña);

            if (usuarioAutenticado != null && usuarioAutenticado.estado)
            {
                Session["Usuario"] = usuarioAutenticado.nickname;
                Session["UsuarioID"] = usuarioAutenticado.id;
                Session["Rol"] = usuarioAutenticado.rol;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    usuarioAutenticado.nickname,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(60),
                    true,
                    usuarioAutenticado.rol,
                    FormsAuthentication.FormsCookiePath
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);


                if (usuarioAutenticado.rol == EntityLayer.GaleriaArte.Util.Constants.ID_USUARIO_COMPRADOR)
                {
                    Response.Redirect("/View/comprador/Panel.aspx");
                }
                else if (usuarioAutenticado.rol == EntityLayer.GaleriaArte.Util.Constants.ID_USUARIO_ARTISTA)
                {
                    Response.Redirect("/View/artista/Panel.aspx");
                }
                else if (usuarioAutenticado.rol == EntityLayer.GaleriaArte.Util.Constants.ID_USUARIO_ADMIN)
                {
                    Response.Redirect("/View/admin/Panel.aspx");
                }
            }
            else
            {
                lblMensaje.Text = "Usuario o contraseña incorrectos, o cuenta deshabilitada.";
            }
        }

        protected void btnRecuperarCuenta_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReestablecerContraseña.aspx");
        }

        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registrarse.aspx");
        }
    }
}
