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
    public class AddLike
    {
        public class AddLikeCommand : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
        }

        public class AddLikeHandler : IRequestHandler<AddLikeCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public AddLikeHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(AddLikeCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.CollectionId == request.CollectionId && l.UserId == user.Id, cancellationToken);

                if (existingLike != null)
                    return Result<Unit>.Failure("Polubienie już istnieje");

                var like = new Like
                {
                    CollectionId = request.CollectionId,
                    UserId = user.Id
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}
