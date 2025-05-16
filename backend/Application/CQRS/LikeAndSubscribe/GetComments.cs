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

namespace Application.CQRS.LikeAndSubscribe
{
    public class GetComments
    {
        public class GetCommentsQuery : IRequest<List<CommentDTO>>
        {
            public Guid CollectionId { get; set; }
        }

        public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, List<CommentDTO>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCommentsHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<List<CommentDTO>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                return await _context.Comments
                    .Where(c => c.CollectionId == request.CollectionId)
                    .Include(c => c.Author)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDTO
                    {
                        CommentId = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        Author = new AuthorDto
                        {
                            Id = c.Author.Id,
                            Email = c.Author.Email,
                            Name = c.Author.Name
                        }
                    })
                    .ToListAsync(cancellationToken);
            }
        }

    }
}
