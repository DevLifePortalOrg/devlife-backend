// DevLife.Api/Hubs/GameHub.cs
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent; // For storing connected users
using System.Linq;

namespace DevLife.Api.Hubs
{
    public class GameHub : Hub
    {
        // A simple in-memory store for connected users and their IDs.
        // In a production environment, consider a persistent store like Redis for scale-out.
        private static ConcurrentDictionary<string, Guid> _connectedUsers = new ConcurrentDictionary<string, Guid>();

        // Method for clients to join a specific game session or group
        public async Task JoinGameSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Anonymous", Context.ConnectionId);
        }

        // Method for clients to leave a game session or group
        public async Task LeaveGameSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Anonymous", Context.ConnectionId);
        }

        // Method for sending general chat messages
        public async Task SendMessage(string user, string message)
        {
            // Send to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Method for sending private messages (e.g., in DevDating chat)
        public async Task SendPrivateMessage(string receiverConnectionId, string message)
        {
            // You would typically map userId to connectionId in a real application
            // For demonstration, directly use connectionId
            await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", Context.User?.Identity?.Name ?? "Anonymous", message);
        }

        // Method to notify specific user about a game event (e.g., "BugSolved", "CasinoResult")
        public async Task SendGameUpdate(Guid userId, string eventType, object payload)
        {
            // Find the connection ID for the user
            var userConnectionId = _connectedUsers.FirstOrDefault(x => x.Value == userId).Key;
            if (!string.IsNullOrEmpty(userConnectionId))
            {
                await Clients.Client(userConnectionId).SendAsync(eventType, payload);
            }
            else
            {
                // Log or handle case where user is not connected
                Console.WriteLine($"User {userId} not found in connected users for {eventType} update.");
            }
        }

        // Method to send a real-time notification (e.g., new match in Dev Dating)
        public async Task SendNotification(Guid userId, string notificationType, string message)
        {
            var userConnectionId = _connectedUsers.FirstOrDefault(x => x.Value == userId).Key;
            if (!string.IsNullOrEmpty(userConnectionId))
            {
                await Clients.Client(userConnectionId).SendAsync("ReceiveNotification", notificationType, message);
            }
        }

        // Called when a new connection is established
        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _connectedUsers.TryAdd(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} connected with connection ID {Context.ConnectionId}");
                // Optionally notify all connected clients about a new user
                await Clients.All.SendAsync("UserConnected", Context.User.Identity.Name);
            }
            else
            {
                Console.WriteLine($"Anonymous user connected with connection ID {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        // Called when a connection is disconnected
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectedUsers.TryRemove(Context.ConnectionId, out Guid userId);
            Console.WriteLine($"User {userId} disconnected from connection ID {Context.ConnectionId}");
            // Optionally notify all connected clients about a user leaving
            await Clients.All.SendAsync("UserDisconnected", Context.User?.Identity?.Name);

            await base.OnDisconnectedAsync(exception);
        }
    }
}