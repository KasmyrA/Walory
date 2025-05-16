using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.LikeAndSubscribe
{
    public class AddComment
    {
        public class AddCommentCommand : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
            public string Content { get; set; }
        }

        public class AddCommentHandler : IRequestHandler<AddCommentCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public AddCommentHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var comment = new Comment
                {
                    CollectionId = request.CollectionId,
                    AuthorId = user.Id,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                var success = await _context.SaveChangesAsync() > 0;
                return success
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Not Added Comment");
            }
        }

    }
}
