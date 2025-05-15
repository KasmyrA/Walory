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
    public class RemoveFriend
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

                var entries = await _context.UserFriends
                    .Where(uf =>
                        (uf.UserId == user.Id && uf.FriendId == request.FriendId) ||
                        (uf.UserId == request.FriendId && uf.FriendId == user.Id))
                    .ToListAsync();

                if (entries.Count == 0) return Result<Unit>.Failure("You aren't friends!");

                _context.UserFriends.RemoveRange(entries);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Your relation will be eternal");
            }
        }
    }

}
