<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.comprador.Panel" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Panel de comprador</title>
    <link rel="stylesheet" type="text/css" href="../css/PanelComprador_styles.css" />
</head>
<body>
    <div class="background-overlay"></div>
    <form id="form1" runat="server">
        <div class="panel-box">
            <asp:Label ID="lblWelcome" runat="server" Font-Size="Large"  CssClass="welcome-label" Font-Bold="true" ForeColor="#4a5568" />
            <div class="top-bar">
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn btn-logout" OnClick="btnCerrarSesion_Click" />
            </div>
            <asp:Repeater ID="rptObras" runat="server">
                <HeaderTemplate>
                    <div class="contenedor-obras">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="tarjeta-obra">
                        <asp:Image ID="imgObra" runat="server" ImageUrl='<%# Eval("archivo_url") %>' AlternateText="Imagen de la obra" />
                        <h3><%# Eval("titulo") %></h3>
                        <p><strong>Precio:</strong> $<%# Eval("precio", "{0:N2}") %></p>
                        <asp:Button ID="btnComprar" runat="server" Text="Comprar" CommandArgument='<%# Eval("id") %>' CssClass="btn btn-primary" />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
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
