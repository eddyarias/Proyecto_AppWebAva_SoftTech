<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecupearCuenta.aspx.cs" Inherits="WebAppGaleriaArte.View.Recover" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Recuperar Cuenta</title>
    <link rel="stylesheet" type="text/css" href="css/RecuperarCuenta_styles.css">
</head>
<body>
    <form id="form1" runat="server">
        <div class="recover-container">
            <h2>Recuperar Cuenta</h2>
            <p class="info-text">Ingresa tu correo electrónico y te enviaremos instrucciones para recuperar tu cuenta.</p>
            <asp:Label ID="lblMensaje" runat="server" CssClass="message-error" />
    
            <div class="form-group">
                <asp:Label runat="server" Text="Correo electrónico:" />
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" />
            </div>
    
            <div class="button-group">
                <asp:Button ID="btnRecuperar" runat="server" Text="Recuperar" OnClick="btnRecuperar_Click" CssClass="btn btn-primary" />
                <asp:Button ID="btnVolver" runat="server" Text="Volver" OnClick="btnVolver_Click" CssClass="btn btn-secondary" />
            </div>
        </div>
    </form>
</body>
</html>
