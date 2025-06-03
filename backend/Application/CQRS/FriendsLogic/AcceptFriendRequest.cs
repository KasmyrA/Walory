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

namespace Application.CQRS.FriendsLogic
{
    public class AcceptFriendRequest
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid RequestId { get; set; }
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
                var notifcation = await _context.Notifications.FirstOrDefaultAsync(n => n.ReferenceId == request.RequestId);
                if (notifcation == null)
                    return Result<Unit>.Failure("Not found");
                var friendRequest = await _context.FriendRequests
                    .FirstOrDefaultAsync(fr => fr.Id == request.RequestId && fr.ReceiverId == notifcation.ReferenceId);

                if (friendRequest == null)
                    return Result<Unit>.Failure("Not found");

                friendRequest.IsAccepted = true;

                _context.UserFriends.AddRange(
                    new UserFriend { UserId = friendRequest.SenderId, FriendId = friendRequest.ReceiverId },
                    new UserFriend { UserId = friendRequest.ReceiverId, FriendId = friendRequest.SenderId }
                );

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Couldn't accept");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
