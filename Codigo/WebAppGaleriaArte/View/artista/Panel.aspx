<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.artista.Panel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Panel Artista</title>
    <link rel="stylesheet" type="text/css" href="css/Panel_styles.css" />

</head>
<body>
    <!-- Overlay para oscurecer y desenfocar el fondo -->
    <div class="background-overlay"></div>
    <form id="form1" runat="server">
        <div class="panel-box">
            <!-- Botón cerrar sesión arriba a la derecha -->
            <div class="top-bar">
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar Sesión" CssClass="btn btn-danger btn-logout" OnClick="btnCerrarSesion_Click" />
            </div>
            <div>
                <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-label" Font-Size="Large" Font-Bold="true" />
            </div>
            <asp:Label
                ID="lblMensajeExito"
                runat="server"
                ForeColor="Green"
                Font-Bold="true"
                EnableViewState="false" />

            <div class="actions-bar">
                <asp:Button
                    ID="btnCrearObra"
                    runat="server"
                    Text="Crear Nueva Obra"
                    OnClick="btnCrearObra_Click"
                    CssClass="btn btn-primary" />

                <asp:TextBox
                    ID="txtBuscarTitulo"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Buscar por título..." />

                <asp:Button
                    ID="btnBuscarTitulo"
                    runat="server"
                    Text="🔍 Buscar"
                    CssClass="btn btn-info" 
                    OnClick="btnBuscarTitulo_Click" />

                <asp:Button
                    ID="btnLimpiarBusqueda"
                    runat="server"
                    Text="Limpiar"
                    CssClass="btn btn-secondary"
                    OnClick="btnLimpiarBusqueda_Click" />
            </div>

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

                            <asp:Button
                                ID="btnEliminar"
                                runat="server"
                                Text="🗑️"
                                Visible="false"
                                CssClass="btn btn-outline-danger btn-sm"
                                ToolTip="Eliminar obra" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <!-- Botón para abrir el chat -->
        <div id="chatButton">💬</div>

        <!-- Ventana del chat -->
        <div id="chatWindow">
            <div id="chatHeader">
                Chat en línea
                <span id="closeChat">&times;</span>
            </div>
            <ul id="chatMessages"></ul>
            <div id="chatFooter">
                <input type="text" id="userName" value="<%= Session["Usuario"] %>" readonly />
                <input type="text" id="messageInput" placeholder="Escribe un mensaje..." />
                <button type="button" id="sendMessage">Enviar</button>
            </div>
        </div>
    </form>
    <!-- SignalR Scripts -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="/Scripts/jquery.signalR-2.4.3.min.js"></script>
    <script src="/signalr/hubs"></script>
    <script>
        $(function () {
            const userName = $('#userName').val();

            $('#chatButton').click(function () {
                $('#chatWindow').show();
                $('#chatMessages').scrollTop(0); // Mostrar último mensaje
            });

            $('#closeChat').click(function () {
                $('#chatWindow').hide();
            });

            $('#messageInput').keydown(function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    $('#sendMessage').click();
                }
            });

            function saveMessage(name, message) {
                let history = JSON.parse(localStorage.getItem('chatHistory') || '[]');
                history.push({ name, message });
                localStorage.setItem('chatHistory', JSON.stringify(history));
            }

            function loadHistory() {
                $('#chatMessages').empty();
                let history = JSON.parse(localStorage.getItem('chatHistory') || '[]');
                history.reverse().forEach(function (msg) {
                    appendMessage(msg.name, msg.message);
                });
            }

            function appendMessage(name, message) {
                const encodedName = $('<div />').text(name).html();
                const encodedMsg = $('<div />').text(message).html();
                const isOwn = name === userName;
                const cssClass = isOwn ? 'chat-message own-message' : 'chat-message other-message';

                // Avatar con la inicial del usuario
                const initial = encodedName.charAt(0).toUpperCase();
                const avatar = `<div class="avatar">${initial}</div>`;

                // Nombre solo para mensajes de otros usuarios
                const nameTag = !isOwn ? `<span class="sender-name">${encodedName}</span>` : '';

                const $msg = $(`
                    <li class="${cssClass}">
                        ${avatar}
                        <div class="bubble">
                            ${nameTag}
                            ${encodedMsg}
                        </div>
                    </li>
                `);
                $('#chatMessages').prepend($msg);
}

            //loadHistory();

            const chat = $.connection.chatHub;

            chat.client.broadcastMessage = function (name, message) {
                console.log("Mensaje recibido:", name, message); // <-- debug
                saveMessage(name, message);
                appendMessage(name, message);
            };

            $.connection.hub.start().done(function () {
                $('#sendMessage').click(function () {
                    const msg = $('#messageInput').val();
                    if (msg.trim() !== '') {
                        chat.server.send(userName, msg);
                        $('#messageInput').val('').focus();
                    }
                });
            });
        });
    </script>
</body>
</html>
