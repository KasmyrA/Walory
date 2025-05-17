using Domain;
using Infrastracture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Walory_Backend
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(Guid receiverId, string content)
        {
            var senderId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sentAt = DateTime.UtcNow;

            using var scope = Context.GetHttpContext().RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.Parse(senderId),
                ReceiverId = receiverId,
                Content = content,
                SentAt = sentAt
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                Content = content,
                SentAt = sentAt
            });
        }
        public async Task MarkAsRead(Guid friendId)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return;

            await Clients.User(friendId.ToString()).SendAsync("MessageRead", new
            {
                ReaderId = userId,
                ReadAt = DateTime.UtcNow
            });
        }
    }


}
