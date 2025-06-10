<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IniciarSesion.aspx.cs" Inherits="WebAppGaleriaArte.View.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Iniciar Sesión</title>
    <link rel="stylesheet" type="text/css" href="css/IniciarSesion_styles.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width:300px;margin:auto;padding-top:100px;">
            <div class="login-container">
            <h2>Iniciar Sesión</h2>
            <asp:Label ID="lblMensaje" runat="server" CssClass="message-error" />
    
            <div class="form-group">
                <asp:Label runat="server" Text="Usuario:" />
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" />
            </div>
    
            <div class="form-group">
                <asp:Label runat="server" Text="Contraseña:" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>
    
            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" OnClick="btnLogin_Click" CssClass="btn btn-primary" />
    
            <div class="button-group">
                <asp:Button ID="btnRecuperarCuenta" runat="server" Text="Recuperar cuenta" OnClick="btnRecuperarCuenta_Click" CssClass="btn btn-secondary" />
                <asp:Button ID="btnRegistrarse" runat="server" Text="Registrarse" OnClick="btnRegistrarse_Click" CssClass="btn btn-secondary" />
            </div>
        </div>
        </div>
    </form>
</body>
</html>

