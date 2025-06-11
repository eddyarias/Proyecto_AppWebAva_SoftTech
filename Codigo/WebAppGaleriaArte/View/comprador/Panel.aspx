<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.comprador.Panel" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Chat estilo WhatsApp</title>
    <link rel="stylesheet" type="text/css" href="css/styles.css" />
    <style>
        #chatButton {
            position: fixed;
            bottom: 20px;
            left: 20px;
            background-color: #4a90e2;
            color: white;
            padding: 12px 16px;
            border-radius: 50%;
            cursor: pointer;
            z-index: 1000;
            font-size: 20px;
        }

        #chatWindow {
            position: fixed;
            bottom: 80px;
            left: 20px;
            width: 320px;
            background: white;
            border: 1px solid #ccc;
            border-radius: 8px;
            padding: 10px;
            display: none;
            z-index: 1000;
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
        }

        #closeChat {
            position: absolute;
            top: 8px;
            right: 12px;
            cursor: pointer;
            font-size: 18px;
        }

        #chatMessages {
            display: flex;
            flex-direction: column-reverse; /* Invertir el orden */
            height: 250px;
            overflow-y: auto;
            padding: 10px;
            margin-bottom: 10px;
            list-style: none;
            background-color: #f0f0f0;
            border-radius: 6px;
        }

        .chat-message {
            max-width: 80%;
            margin: 5px 0;
            padding: 8px 12px;
            border-radius: 15px;
            clear: both;
            word-wrap: break-word;
            font-size: 14px;
        }

        .own-message {
            align-self: flex-end;
            background-color: #dcf8c6;
            text-align: right;
        }

        .other-message {
            align-self: flex-start;
            background-color: #e5e5ea;
            text-align: left;
        }

        #userName,
        #messageInput {
            width: 100%;
            margin-bottom: 5px;
            padding: 6px;
            box-sizing: border-box;
            border: 1px solid #ccc;
            border-radius: 6px;
        }

        #sendMessage {
            width: 100%;
            padding: 6px;
            background-color: #4a90e2;
            color: white;
            border: none;
            cursor: pointer;
            border-radius: 6px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lblWelcome" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="#4a5568" />

        <!-- Botón para abrir el chat -->
        <div id="chatButton">💬</div>

        <!-- Ventana del chat -->
        <div id="chatWindow">
            <span id="closeChat">&times;</span>
            <ul id="chatMessages"></ul>
            <input type="text" id="userName" value="<%= Session["Usuario"] %>" readonly />
            <input type="text" id="messageInput" placeholder="Escribe un mensaje..." />
            <button type="button" id="sendMessage">Enviar</button>
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

                const $msg = $('<li class="' + cssClass + '">').html(encodedMsg);
                $('#chatMessages').prepend($msg); // prepend por column-reverse
            }

            loadHistory();

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
