<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Restablecer.aspx.cs" Inherits="WebAppGaleriaArte.View.Restablecer" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Restablecer Contraseña</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" />
            <br />
            <asp:Label ID="lblNueva" runat="server" Text="Nueva contraseña:" />
            <asp:TextBox ID="txtNueva" runat="server" TextMode="Password" />
            <br />
            <asp:Label ID="lblConfirmar" runat="server" Text="Confirmar contraseña:" />
            <asp:TextBox ID="txtConfirmar" runat="server" TextMode="Password" />
            <br />
            <asp:Button ID="btnRestablecer" runat="server" Text="Restablecer" OnClick="btnRestablecer_Click" />
        </div>
    </form>
</body>
</html>
