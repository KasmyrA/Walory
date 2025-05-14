using Application.DTO;
using Domain;
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

namespace Application.CQRS.Notification
{
    public class MarkNotificationAsRead
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid NotificationId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notification = await _context.Notifications.FindAsync(request.NotificationId);

                if (notification == null)
                {
                    throw new Exception("Notifcation not found");
                }

                notification.IsRead = true;

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Not read");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
