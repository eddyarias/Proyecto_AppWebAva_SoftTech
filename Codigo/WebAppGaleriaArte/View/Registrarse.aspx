<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registrarse.aspx.cs" Inherits="WebAppGaleriaArte.View.SingUp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registrarse</title>
    <!-- Bootstrap 5 CDN -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Estilos personalizados -->
    <link rel="stylesheet" type="text/css" href="css/styleRegistrarse.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex align-items-center justify-content-center min-vh-100">
            <div class="register-box shadow-sm p-4 rounded">
                <h2 class="text-center mb-4">Registrarse</h2>

                <asp:Label ID="lblMensaje" runat="server" CssClass="message-error text-danger d-block mb-2" />

                <div class="mb-3">
                    <asp:Label runat="server" Text="Usuario:" CssClass="form-label" />
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" Text="Correo:" CssClass="form-label" />
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" Text="Contraseña:" CssClass="form-label" />
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" Text="Rol:" CssClass="form-label" />
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select" />
                    <div class="form-text text-light">Selecciona el tipo de cuenta que deseas crear</div>
                </div>

                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" OnClick="btnRegistrar_Click" CssClass="btn btn-primary w-100 mb-3" />

                <asp:Button ID="btnVolver" runat="server" Text="Volver" OnClick="btnVolver_Click" CssClass="btn btn-outline-light w-100" />
            </div>
        </div>
    </form>
</body>
</html>
