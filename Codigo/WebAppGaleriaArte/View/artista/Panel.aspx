<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.Panel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Panel Artista</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblWelcome" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="#4a5568" />
        </div>
        <asp:Label 
            ID="lblMensajeExito" 
            runat="server" 
            ForeColor="Green" 
            Font-Bold="true" 
            EnableViewState="false" />

        <div style="margin-top: 20px;">
            <asp:Button
                ID="btnCrearObra"
                runat="server"
                Text="Crear Nueva Obra"
                OnClick="btnCrearObra_Click"
                CssClass="btn btn-primary"
                Style="margin-bottom: 15px;" />
            <asp:GridView
                ID="gvObras"
                runat="server"
                AutoGenerateColumns="False"
                EmptyDataText="No hay obras registradas."
                CssClass="table table-bordered"
                HeaderStyle-BackColor="#4a5568"
                HeaderStyle-ForeColor="White"
                OnRowCommand="gvObras_RowCommand"
                OnRowDataBound="gvObras_RowDataBound">

                <Columns>
                    <asp:ImageField DataImageUrlField="archivo_url" HeaderText="Imagen" ControlStyle-Width="100px" ControlStyle-Height="100px" />
                    <asp:BoundField DataField="titulo" HeaderText="Título" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                    <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="estado" HeaderText="Estado" />
                    <asp:BoundField DataField="fecha_publicacion" HeaderText="Fecha de Publicación" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button
                                ID="btnEditar"
                                runat="server"
                                Text="Editar"
                                CommandName="EditarObra"
                                CommandArgument='<%# Eval("id") %>'
                                CssClass="btn btn-warning btn-sm" />

                            <asp:Button
                                ID="btnOcultar"
                                runat="server"
                                Text="Ocultar"
                                CommandName="OcultarObra"
                                CommandArgument='<%# Eval("id") %>'
                                CssClass="btn btn-danger btn-sm"
                                OnClientClick="return confirm('¿Estás seguro que deseas ocultar esta obra?');" />

                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
