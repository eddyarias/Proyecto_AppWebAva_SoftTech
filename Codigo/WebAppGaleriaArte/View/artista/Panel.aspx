<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.Panel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Panel Artista</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblWelcome" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="#4a5568" />
        </div>
        <div style="margin-top:20px;">
            <asp:Button 
                ID="btnCrearObra" 
                runat="server" 
                Text="Crear Nueva Obra" 
                OnClick="btnCrearObra_Click" 
                CssClass="btn btn-primary"
                style="margin-bottom:15px;" />
            <asp:GridView 
                ID="gvObras" 
                runat="server" 
                AutoGenerateColumns="False" 
                EmptyDataText="No hay obras registradas."
                CssClass="table table-bordered"
                HeaderStyle-BackColor="#4a5568"
                HeaderStyle-ForeColor="White">
                <Columns>
                    <asp:ImageField DataImageUrlField="archivo_url" HeaderText="Imagen" ControlStyle-Width="100px" ControlStyle-Height="100px" />
                    <asp:BoundField DataField="titulo" HeaderText="Título" />
                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                    <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="estado" HeaderText="Estado" />
                    <asp:BoundField DataField="fecha_publicacion" HeaderText="Fecha de Publicación" DataFormatString="{0:dd/MM/yyyy}" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
