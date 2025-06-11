<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IniciarSesion.aspx.cs" Inherits="WebAppGaleriaArte.View.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Iniciar Sesión</title>
    <!-- Bootstrap 5 CDN -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Estilos personalizados -->
    <link rel="stylesheet" type="text/css" href="css/styleIniciarSesion.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex align-items-center justify-content-center min-vh-100">
            <div class="login-box shadow-sm p-4 rounded">
                <h2 class="text-center mb-4">Iniciar Sesión</h2>

                <asp:Label ID="lblMensaje" runat="server" CssClass="message-error text-danger d-block mb-2" />

                <div class="mb-3">
                    <asp:Label runat="server" Text="Usuario:" CssClass="form-label" />
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" Text="Contraseña:" CssClass="form-label" />
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Ingresar" OnClick="btnLogin_Click" CssClass="btn btn-primary w-100 mb-3" />

                <div class="d-flex justify-content-between">
                    <asp:Button ID="btnRecuperarCuenta" runat="server" Text="Recuperar cuenta" OnClick="btnRecuperarCuenta_Click" CssClass="btn btn-outline-light btn-sm" />
                    <asp:Button ID="btnRegistrarse" runat="server" Text="Registrarse" OnClick="btnRegistrarse_Click" CssClass="btn btn-outline-light btn-sm" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
