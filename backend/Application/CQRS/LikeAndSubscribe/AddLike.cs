using Domain;
using MediatR;
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
            private readonly AppDbContext _context;
            private readonly IUserContextService _userContext;

            public AddLikeHandler(AppDbContext context, IUserContextService userContext)
            {
                _context = context;
                _userContext = userContext;
            }

            public async Task<Result<Unit>> Handle(AddLikeCommand request, CancellationToken cancellationToken)
            {
                var userId = _userContext.UserId;
                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.CollectionId == request.CollectionId && l.UserId == userId, cancellationToken);

                if (existingLike != null)
                    return Result<Unit>.Failure("Polubienie już istnieje");

                var like = new Like
                {
                    Id = Guid.NewGuid(),
                    CollectionId = request.CollectionId,
                    UserId = userId
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}
