using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notificaiton
{
    public class FriendRequest
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsAccepted { get; set; } = false;
    }
}
