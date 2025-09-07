using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatApp.Api.Data;
using Microsoft.EntityFrameworkCore;
using ChatApp.Api.Models;

namespace ChatApp.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        private static readonly Dictionary<string, string> _connections = new();

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(userId))
            {
                // Store connection
                _connections[Context.ConnectionId] = username;

                // Update user online status
                if (int.TryParse(userId, out int id))
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user != null)
                    {
                        user.IsOnline = true;
                        await _context.SaveChangesAsync();
                    }
                }

                // Send current online users to the new connection
                var onlineUsers = _connections.Values.Distinct().ToList();
                await Clients.Caller.SendAsync("CurrentUsers", onlineUsers);

                // Notify others about new user
                await Clients.Others.SendAsync("UserConnected", username);

                Console.WriteLine($"User connected: {username}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var username))
            {
                _connections.Remove(Context.ConnectionId);

                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user != null)
                    {
                        user.IsOnline = false;
                        await _context.SaveChangesAsync();
                    }
                }

                await Clients.Others.SendAsync("UserDisconnected", username);
                Console.WriteLine($"User disconnected: {username}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string content)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
            {
                var message = new Message
                {
                    Content = content,
                    UserId = id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                await Clients.All.SendAsync("ReceiveMessage", new
                {
                    id = message.Id,
                    content = message.Content,
                    userId = message.UserId,
                    username = username,
                    createdAt = message.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                });
            }
        }

        // NEW: Reaction methods
        public async Task AddReaction(int messageId, string emoji)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
            {
                var existingReaction = await _context.MessageReactions
                    .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == id && r.Emoji == emoji);

                if (existingReaction == null)
                {
                    var reaction = new MessageReaction
                    {
                        MessageId = messageId,
                        UserId = id,
                        Emoji = emoji,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.MessageReactions.Add(reaction);
                    await _context.SaveChangesAsync();

                    await Clients.All.SendAsync("ReactionAdded", new
                    {
                        messageId = messageId,
                        emoji = emoji,
                        username = username,
                        userId = id
                    });
                }
            }
        }

        public async Task RemoveReaction(int messageId, string emoji)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id))
            {
                var reaction = await _context.MessageReactions
                    .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == id && r.Emoji == emoji);

                if (reaction != null)
                {
                    _context.MessageReactions.Remove(reaction);
                    await _context.SaveChangesAsync();

                    await Clients.All.SendAsync("ReactionRemoved", new
                    {
                        messageId = messageId,
                        emoji = emoji,
                        userId = id
                    });
                }
            }
        }
    }
}