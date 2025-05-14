using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }          
        public User User { get; set; }

        public NotificationType Type { get; set; }
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum NotificationType
    {
        FriendRequest,
        MessageSent
    }
}
