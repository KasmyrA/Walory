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

namespace Application.CQRS.LikeAndSubscribe
{
    public class ChecIfLiked
    {
        public class ChecIfLikedCommand : IRequest<Result<bool>>
        {
            public Guid CollectionId { get; set; }
        }

        public class ChecIfLikedHandler : IRequestHandler<ChecIfLikedCommand, Result<bool>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public ChecIfLikedHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<bool>> Handle(ChecIfLikedCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var like = await _context.Likes.Where(c => c.CollectionId == request.CollectionId && c.UserId == user.Id).AnyAsync();

                if (like == true)
                    return Result<bool>.Success(true);
                return  Result<bool>.Success(false);


            }
        }

    }
}
