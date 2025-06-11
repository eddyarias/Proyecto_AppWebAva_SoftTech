<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registrarse.aspx.cs" Inherits="WebAppGaleriaArte.View.SingUp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registrarse</title>
    <link rel="stylesheet" type="text/css" href="css/Registrarse_styles.css">
</head>
<body>
    <form id="form1" runat="server">
       <div class="register-container">
            <h2>Registrarse</h2>
            <asp:Label ID="lblMensaje" runat="server" CssClass="message-error" />
    
            <div class="form-group">
                <asp:Label runat="server" Text="Usuario:" CssClass="required" />
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" />
            </div>
    
            <div class="form-group">
                <asp:Label runat="server" Text="Correo:" CssClass="required" />
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" />
            </div>
    
            <div class="form-group">
                <asp:Label runat="server" Text="Contraseña:" CssClass="required" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>
    
            <div class="form-group">
                <asp:Label runat="server" Text="Rol:" CssClass="required" />
                <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Comprador" Value="comprador" />
                    <asp:ListItem Text="Artista" Value="artista" />
                    <asp:ListItem Text="Admin" Value="admin" />
                </asp:DropDownList>
                <div class="role-info">Selecciona el tipo de cuenta que deseas crear</div>
            </div>
    
            <div class="button-group">
                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" OnClick="btnRegistrar_Click" CssClass="btn btn-primary" />
                <asp:Button ID="btnVolver" runat="server" Text="Volver" OnClick="btnVolver_Click" CssClass="btn btn-secondary" />
            </div>
        </div>
    </form>
</body>
</html>
