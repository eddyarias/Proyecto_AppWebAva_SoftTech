<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="WebAppGaleriaArte.View.comprador.Panel" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Panel de comprador</title>
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

        body {
            background-color: #121212;
            color: #e7e9ea;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
        }

        /* Botón de cerrar sesión en rojo */
        .btn-logout {
            background-color: #e0245e;
            border: none;
            color: #fff;
            font-weight: 600;
            font-size: 15px;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            box-shadow: 0 2px 8px rgba(224,36,94,0.3);
            transition: background 0.2s;
        }

            .btn-logout:hover {
                background-color: #c81e4a;
            }

        /* Contenedor principal de obras */
        .contenedor-obras {
            display: flex;
            flex-wrap: wrap;
            gap: 24px;
            justify-content: center;
            padding: 24px;
        }

        /* Tarjeta de cada obra */
        .tarjeta-obra {
            background-color: #1e1e1e;
            border-radius: 16px;
            padding: 16px;
            width: 220px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.4);
            text-align: center;
            transition: transform 0.2s;
        }

            .tarjeta-obra:hover {
                transform: translateY(-4px);
            }

            /* Imagen de obra con tamaño uniforme */
            .tarjeta-obra img {
                width: 180px;
                height: 180px;
                object-fit: cover;
                border-radius: 12px;
                border: 2px solid #4a90e2;
                margin-bottom: 12px;
                background-color: #f0f4f8;
            }

            /* Títulos */
            .tarjeta-obra h3 {
                font-size: 16px;
                margin: 8px 0;
                color: #ffffff;
            }

            /* Precio */
            .tarjeta-obra p {
                font-size: 14px;
                color: #ccc;
                margin-bottom: 12px;
            }

            /* Botón Comprar */
            .tarjeta-obra .btn {
                background-color: #1d9bf0;
                border: none;
                color: white;
                padding: 8px 16px;
                border-radius: 8px;
                font-weight: 600;
                cursor: pointer;
                transition: background 0.2s;
            }

                .tarjeta-obra .btn:hover {
                    background-color: #1a8cd8;
                }

        /* Barra superior de acciones (como cerrar sesión) */
        .top-bar {
            display: flex;
            justify-content: flex-end;
            padding: 16px 24px;
        }

        #lblWelcome {
            font-size: 1.25rem; /* Tamaño grande */
            font-weight: bold; /* Negrita */
            color: #4a5568; /* Color ForeColor */
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lblWelcome" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="#4a5568" />
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
