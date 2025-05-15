using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.LikeAndSubscribe
{
    public class DeleteCommand
    {
        public class DeleteCommentCommand : IRequest<Result<Unit>>
        {
            public Guid CommentId { get; set; }
        }

        public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<Unit>>
        {
            private readonly AppDbContext _context;
            private readonly IUserContextService _userContext;

            public DeleteCommentHandler(AppDbContext context, IUserContextService userContext)
            {
                _context = context;
                _userContext = userContext;
            }

            public async Task<Result<Unit>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var userId = _userContext.UserId;
                var comment = await _context.Comments.FindAsync(request.CommentId);

                if (comment == null || comment.AuthorId != userId)
                    return Result<Unit>.Failure("Nie znaleziono komentarza lub brak uprawnień");

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}
