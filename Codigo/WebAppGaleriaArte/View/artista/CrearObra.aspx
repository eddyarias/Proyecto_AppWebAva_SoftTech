<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearObra.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.CrearObra" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crear Obra</title>
    <!-- Bootstrap 5 CDN -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Estilos personalizados -->
    <link rel="stylesheet" type="text/css" href="../css/styleCrearObra.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container d-flex align-items-center justify-content-center min-vh-100">
            <div class="crear-obra-box shadow-sm p-4 rounded">
                <asp:Button
                    ID="btnRegresar"
                    runat="server"
                    Text="← Regresar"
                    CssClass="btn btn-outline-light btn-sm mb-3"
                    OnClick="btnRegresar_Click"
                    CausesValidation="false" />

                <h2 class="text-center mb-4">Registrar Nueva Obra</h2>

                <asp:Label ID="lblMensaje" runat="server" CssClass="message-error text-danger d-block mb-2" />

                <asp:ValidationSummary
                    ID="valSummary"
                    runat="server"
                    CssClass="text-danger mb-3"
                    ShowMessageBox="false"
                    ShowSummary="true" />

                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="txtTitulo" Text="Título:" CssClass="form-label" />
                    <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="255" />
                    <asp:RequiredFieldValidator
                        ID="rfvTitulo"
                        runat="server"
                        ControlToValidate="txtTitulo"
                        ErrorMessage="El campo Título es obligatorio."
                        Text="*"
                        Display="Dynamic"
                        CssClass="text-danger" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="txtDescripcion" Text="Descripción:" CssClass="form-label" />
                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="fupArchivo" Text="Archivo:" CssClass="form-label" />
                    <asp:FileUpload ID="fupArchivo" runat="server" CssClass="form-control" />
                </div>

                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="txtPrecio" Text="Precio:" CssClass="form-label" />
                    <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator
                        ID="rfvPrecio"
                        runat="server"
                        ControlToValidate="txtPrecio"
                        ErrorMessage="El campo Precio es obligatorio."
                        Text="*"
                        Display="Dynamic"
                        CssClass="text-danger" />
                    <asp:RegularExpressionValidator
                        ID="revPrecio"
                        runat="server"
                        ControlToValidate="txtPrecio"
                        ErrorMessage="El formato del precio es inválido. Ej: 99.99"
                        Text="!"
                        Display="Dynamic"
                        ValidationExpression="^\d+(\.\d{1,2})?$"
                        CssClass="text-danger" />
                </div>

                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary w-100" />
            </div>
        </div>
    </form>
</body>
</html>
