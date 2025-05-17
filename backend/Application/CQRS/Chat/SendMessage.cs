using Domain;
using Domain.Notificaiton;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Chat
{
    public class SendMessage
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ReceiverId { get; set; }
            public string Content { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;

            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var message = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = user.Id,
                    ReceiverId = request.ReceiverId,
                    Content = request.Content,
                    SentAt = DateTime.UtcNow
                };
                var notification = new Domain.Notificaiton.Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = request.ReceiverId,
                    Type = NotificationType.MessageSent,
                    Message = $"Nowa wiadomość od {user.Name}",
                    ReferenceId = message.Id
                };

                _context.Notifications.Add(notification);
                _context.Messages.Add(message);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Couldn't send");
            }
        }
    }

}
