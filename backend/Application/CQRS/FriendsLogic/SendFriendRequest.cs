using Domain;
using Domain.Notificaiton;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.CQRS.FriendsLogic
{
    public class SendFriendRequest
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string ReceiverEmail { get; set; }
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
                var sender = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var receiver = await _userManager.FindByEmailAsync(request.ReceiverEmail);

                if (receiver == null || sender.Id == receiver.Id)
                    return Result<Unit>.Failure("Bad user");

                var alreadyFriend = await _context.UserFriends.AnyAsync(uf =>
                    (uf.UserId == sender.Id && uf.FriendId == receiver.Id) ||
                    (uf.UserId == receiver.Id && uf.FriendId == sender.Id));

                if (alreadyFriend)
                    return Result<Unit>.Failure("User is now your friends");

                var existingRequest = await _context.FriendRequests.AnyAsync(fr =>
                    fr.SenderId == sender.Id && fr.ReceiverId == receiver.Id && !fr.IsAccepted);

                if (existingRequest)
                    return Result<Unit>.Failure("Invitation has been sent");

                var friendRequest = new FriendRequest
                {
                    SenderId = sender.Id,
                    ReceiverId = receiver.Id
                };

                _context.FriendRequests.Add(friendRequest);

                await _context.SaveChangesAsync();

                _context.Notifications.Add(new Domain.Notificaiton.Notification
                {
                    UserId = receiver.Id,
                    Type = NotificationType.FriendRequest,
                    Message = $"{sender.Name} wysłał Ci zaproszenie.",
                    ReferenceId = friendRequest.Id
                });

                var success = await _context.SaveChangesAsync() > 0;
                return success
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Failure can't send an invitation");
            }
        }
    }

}
