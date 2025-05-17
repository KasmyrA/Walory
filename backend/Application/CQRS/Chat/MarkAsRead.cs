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
using static System.Net.WebRequestMethods;

namespace Application.CQRS.Chat
{
    public class MarkMessagesAsRead
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

                var messages = await _context.Messages
                    .Where(m => m.ReceiverId == user.Id && m.SenderId == request.FriendId && !m.IsRead)
                    .ToListAsync();

                if (!messages.Any()) return Result<Unit>.Success(Unit.Value);

                foreach (var m in messages)
                    m.IsRead = true;

                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
