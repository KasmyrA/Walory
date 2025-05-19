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
    public class RemoveLikeCommand : IRequest<Result<Unit>>
    {
        public Guid CollectionId { get; set; }
    }

    public class RemoveLikeHandler : IRequestHandler<RemoveLikeCommand, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RemoveLikeHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Unit>> Handle(RemoveLikeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.CollectionId == request.CollectionId && l.UserId == user.Id, cancellationToken);

            if (like == null)
                return Result<Unit>.Failure("Liked not found");

            _context.Likes.Remove(like);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
        }
    }

}
