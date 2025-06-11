<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearObra.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.CrearObra" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Crear Obra</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="max-width: 500px; margin: auto;">
            <h2>Registrar Nueva Obra</h2>
            <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" />
            <table>
                <tr>
                    <td>Título:</td>
                    <td>
                        <asp:TextBox ID="txtTitulo" runat="server" MaxLength="255" />
                        <asp:RequiredFieldValidator ID="rfvTitulo" runat="server" ControlToValidate="txtTitulo" ErrorMessage="*" ForeColor="Red" />
                    </td>
                </tr>
                <tr>
                    <td>Descripción:</td>
                    <td>
                        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" Columns="30" />
                    </td>
                </tr>
                <tr>
                    <td>Archivo:<br />
                        <asp:FileUpload ID="fupArchivo" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>Precio:</td>
                    <td>
                        <asp:TextBox ID="txtPrecio" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" ControlToValidate="txtPrecio" ErrorMessage="*" ForeColor="Red" />
                        <asp:RegularExpressionValidator ID="revPrecio" runat="server" ControlToValidate="txtPrecio" ErrorMessage="N° inválido" ValidationExpression="^\d+(\.\d{1,2})?$" ForeColor="Red" />
                    </td>
                </tr>
                <tr>
                    <td>Estado:</td>
                    <td>
                        <asp:DropDownList ID="ddlEstado" runat="server">
                            <asp:ListItem Text="Activa" Value="activa" />
                            <asp:ListItem Text="Vendida" Value="vendida" />
                            <asp:ListItem Text="Oculta" Value="oculta" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center; padding-top: 10px;">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                    </td>
                </tr>

            </table>
        </div>
    </form>
</body>
</html>
