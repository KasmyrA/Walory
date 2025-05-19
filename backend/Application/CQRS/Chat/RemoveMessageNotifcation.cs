using Domain;
using Domain.Notificaiton;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Chat
{
    public class RemoveMessageNotifications
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid FriendId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor http)
            {
                _context = context;
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var notifications = await _context.Notifications
                    .Where(n => n.UserId == user.Id &&
                                n.Type == NotificationType.MessageSent &&
                                _context.Messages.Any(m => m.Id == n.ReferenceId &&
                                                               m.SenderId == request.FriendId &&
                                                               m.ReceiverId == user.Id &&
                                                               m.IsRead))
                    .ToListAsync();

                if (!notifications.Any()) return Result<Unit>.Success(Unit.Value);

                _context.Notifications.RemoveRange(notifications);

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
            }
        }
    }

}
