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

namespace Application.CQRS.FriendsLogic
{
    public class GetFriendsList
    {
        public class Query : IRequest<Result<List<FriendDTO>>> { }

        public class Handler : IRequestHandler<Query, Result<List<FriendDTO>>>
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

            public async Task<Result<List<FriendDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var friends = await _context.UserFriends
                    .Where(f => f.UserId == user.Id)
                    .Include(f => f.Friend)
                    .Select(f => new FriendDTO
                    {
                        Id = f.Friend.Id,
                        Email = f.Friend.Email,
                        FullName = f.Friend.Name
                    })
                    .ToListAsync();

                return Result<List<FriendDTO>>.Success(friends);
            }
        }

    }

}
