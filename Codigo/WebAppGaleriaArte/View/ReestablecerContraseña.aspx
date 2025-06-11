<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReestablecerContraseña.aspx.cs" Inherits="WebAppGaleriaArte.View.Recover" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Recuperar Cuenta</title>
    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Estilos personalizados -->
    <link rel="stylesheet" type="text/css" href="css/styleRestablecerContraseña.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex align-items-center justify-content-center min-vh-100">
            <div class="recover-box shadow-sm p-4 rounded">
                <h2 class="text-center mb-3">Recuperar Cuenta</h2>
                <p class="info-text text-light mb-3">Ingresa tu correo electrónico y te enviaremos instrucciones para recuperar tu cuenta.</p>

                <asp:Label ID="lblMensaje" runat="server" CssClass="message-error text-danger d-block mb-2" />

                <div class="mb-3">
                    <asp:Label runat="server" Text="Correo electrónico:" CssClass="form-label" />
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" />
                </div>

                <asp:Button ID="btnRecuperar" runat="server" Text="Recuperar" OnClick="btnRecuperar_Click" CssClass="btn btn-primary w-100 mb-3" />

                <asp:Button ID="btnVolver" runat="server" Text="Volver" OnClick="btnVolver_Click" CssClass="btn btn-outline-light w-100" />
            </div>
        </div>
    </form>
</body>
</html>
