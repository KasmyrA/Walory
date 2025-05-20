using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }
        public required User Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public required User Receiver { get; set; }

        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
