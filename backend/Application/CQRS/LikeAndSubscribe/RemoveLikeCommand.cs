using MediatR;
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
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public RemoveLikeHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<Result<Unit>> Handle(RemoveLikeCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.CollectionId == request.CollectionId && l.UserId == userId, cancellationToken);

            if (like == null)
                return Result<Unit>.Failure("Polubienie nie istnieje");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }

}
