using Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.LikeAndSubscribe
{
    public class GetComments
    {
        public class GetCommentsQuery : IRequest<List<CommentDto>>
        {
            public Guid CollectionId { get; set; }
        }

        public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
        {
            private readonly AppDbContext _context;

            public GetCommentsHandler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<List<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                return await _context.Comments
                    .Where(c => c.CollectionId == request.CollectionId)
                    .Include(c => c.Author)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        Author = new AuthorDto
                        {
                            Id = c.Author.Id,
                            Email = c.Author.Email,
                            FullName = c.Author.FullName
                        }
                    })
                    .ToListAsync(cancellationToken);
            }
        }

    }
}
