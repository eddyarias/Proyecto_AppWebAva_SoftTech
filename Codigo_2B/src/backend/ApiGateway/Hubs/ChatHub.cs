using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ApiGateway.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public async Task RegisterUser(string username)
        {
            _userConnections[Context.ConnectionId] = username;
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
        }

        public async Task SendPrivateMessage(string fromUser, string toUser, string message)
        {
            // Encontrar la conexión del usuario destinatario
            var toUserConnectionId = _userConnections.FirstOrDefault(x => x.Value == toUser).Key;
            
            if (!string.IsNullOrEmpty(toUserConnectionId))
            {
                // Enviar al destinatario
                await Clients.Client(toUserConnectionId).SendAsync("privateMessage", fromUser, toUser, message);
                
                // Enviar confirmación al remitente
                await Clients.Caller.SendAsync("privateMessage", fromUser, toUser, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnections.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
