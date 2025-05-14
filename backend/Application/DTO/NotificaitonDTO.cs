using Domain.Notificaiton;

namespace Application.DTO
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? ReferenceId { get; set; }
    }
}
