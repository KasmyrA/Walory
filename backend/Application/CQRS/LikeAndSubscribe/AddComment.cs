using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.LikeAndSubscribe
{
    public class AddComment
    {
        public class AddCommentCommand : IRequest<Result<Guid>>
        {
            public Guid CollectionId { get; set; }
            public string Content { get; set; }
        }

        public class AddCommentHandler : IRequestHandler<AddCommentCommand, Result<Guid>>
        {
            private readonly AppDbContext _context;
            private readonly IUserContextService _userContext;

            public AddCommentHandler(AppDbContext context, IUserContextService userContext)
            {
                _context = context;
                _userContext = userContext;
            }

            public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
            {
                var userId = _userContext.UserId;
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    CollectionId = request.CollectionId,
                    AuthorId = userId,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(comment.Id);
            }
        }

    }
}
