<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.comprador.Panel" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Chat estilo WhatsApp</title>
    <style>
        #chatButton {
            position: fixed;
            bottom: 24px;
            left: 24px;
            background: linear-gradient(135deg, #4a90e2 60%, #357ab8 100%);
            color: white;
            padding: 14px 18px;
            border-radius: 50%;
            cursor: pointer;
            z-index: 1000;
            font-size: 22px;
            box-shadow: 0 4px 16px rgba(74,144,226,0.18);
            transition: background 0.2s;
        }
        #chatButton:hover {
            background: linear-gradient(135deg, #357ab8 60%, #4a90e2 100%);
        }
        #chatWindow {
            position: fixed;
            bottom: 90px;
            left: 24px;
            width: 350px;
            background: #fff;
            border: none;
            border-radius: 16px;
            padding: 0;
            display: none;
            z-index: 1000;
            box-shadow: 0 8px 32px rgba(0,0,0,0.18);
            overflow: hidden;
            font-family: 'Segoe UI', Arial, sans-serif;
        }
        #chatHeader {
            background: linear-gradient(135deg, #4a90e2 60%, #357ab8 100%);
            color: #fff;
            padding: 14px 18px;
            font-weight: bold;
            font-size: 17px;
            letter-spacing: 1px;
            position: relative;
        }
        #closeChat {
            position: absolute;
            top: 10px;
            right: 18px;
            cursor: pointer;
            font-size: 22px;
            color: #fff;
            opacity: 0.7;
            transition: opacity 0.2s;
        }
        #closeChat:hover {
            opacity: 1;
        }
        #chatMessages {
            display: flex;
            flex-direction: column-reverse;
            height: 270px;
            overflow-y: auto;
            padding: 18px 12px 12px 12px;
            margin: 0;
            list-style: none;
            background: #f7fafd;
            border-bottom: 1px solid #e3e3e3;
        }
        .chat-message {
            display: flex;
            align-items: flex-end;
            margin: 8px 0;
            font-size: 15px;
            border-radius: 18px;
            max-width: 85%;
            padding: 0;
            background: none;
            box-shadow: none;
        }
        .own-message {
            align-self: flex-end;
            flex-direction: row-reverse;
        }
        .other-message {
            align-self: flex-start;
        }
        .avatar {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background: linear-gradient(135deg, #4a90e2 60%, #357ab8 100%);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
            margin: 0 8px;
            box-shadow: 0 2px 8px rgba(74,144,226,0.10);
            flex-shrink: 0;
        }
        .bubble {
            padding: 10px 16px;
            border-radius: 18px;
            background: #e5e5ea;
            color: #222;
            position: relative;
            min-width: 60px;
            word-break: break-word;
            box-shadow: 0 2px 8px rgba(0,0,0,0.04);
        }
        .own-message .bubble {
            background: #dcf8c6;
            color: #222;
        }
        .sender-name {
            font-size: 12px;
            font-weight: 600;
            color: #4a90e2;
            margin-bottom: 2px;
            display: block;
            letter-spacing: 0.5px;
        }
        #chatFooter {
            padding: 12px 14px;
            background: #f7fafd;
            display: flex;
            gap: 6px;
            border-top: 1px solid #e3e3e3;
        }
        #userName {
            display: none;
        }
        #messageInput {
            flex: 1;
            padding: 8px 12px;
            border: 1px solid #cfd8dc;
            border-radius: 18px;
            font-size: 15px;
            outline: none;
            background: #fff;
            transition: border 0.2s;
        }
        #messageInput:focus {
            border: 1.5px solid #4a90e2;
        }
        #sendMessage {
            padding: 0 18px;
            background: linear-gradient(135deg, #4a90e2 60%, #357ab8 100%);
            color: white;
            border: none;
            cursor: pointer;
            border-radius: 18px;
            font-size: 15px;
            font-weight: 600;
            transition: background 0.2s;
        }
        #sendMessage:hover {
            background: linear-gradient(135deg, #357ab8 60%, #4a90e2 100%);
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
