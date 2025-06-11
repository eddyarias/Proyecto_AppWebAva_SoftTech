<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditarObra.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.EditarObra" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Editar Obra</title>
</head>
<body>
    <form id="form1" runat="server">

        <div>
            <asp:Button
                ID="btnRegresar"
                runat="server"
                Text="← Regresar"
                CssClass="btn btn-secondary"
                OnClick="btnRegresar_Click"
                CausesValidation="false" />
        </div>

        <div style="max-width: 500px; margin: auto;">
            <h2>Editar Obra</h2>

            <asp:Label ID="lblMensaje" runat="server" ForeColor="Red" />

            <asp:ValidationSummary 
                ID="valSummary" 
                runat="server" 
                ForeColor="Red" 
                HeaderText=""
                ShowMessageBox="false" 
                ShowSummary="true" />

            <table>
                <tr>
                    <td>Título:</td>
                    <td>
                        <asp:TextBox ID="txtTitulo" runat="server" MaxLength="255" />
                        <asp:RequiredFieldValidator 
                            ID="rfvTitulo" 
                            runat="server" 
                            ControlToValidate="txtTitulo" 
                            ErrorMessage="El campo Título es obligatorio." 
                            Text="*" 
                            Display="Dynamic" 
                            ForeColor="Red" />
                    </td>
                </tr>

                <tr>
                    <td>Descripción:</td>
                    <td>
                        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" Columns="30" />
                    </td>
                </tr>

                <tr>
                    <td>Precio:</td>
                    <td>
                        <asp:TextBox ID="txtPrecio" runat="server" />
                        <asp:RequiredFieldValidator 
                            ID="rfvPrecio" 
                            runat="server" 
                            ControlToValidate="txtPrecio" 
                            ErrorMessage="El campo Precio es obligatorio." 
                            Text="*" 
                            Display="Dynamic" 
                            ForeColor="Red" />
                        <asp:RegularExpressionValidator 
                            ID="revPrecio" 
                            runat="server" 
                            ControlToValidate="txtPrecio" 
                            ErrorMessage="El formato del precio es inválido. Ej: 99.99" 
                            Text="!" 
                            Display="Dynamic" 
                            ValidationExpression="^\d+(\.\d{1,2})?$" 
                            ForeColor="Red" />
                    </td>
                </tr>

                <tr>
                    <td colspan="2" style="text-align:center; padding-top:10px;">
                        <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="btnActualizar_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
