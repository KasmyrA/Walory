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
    public class DeleteCommand
    {
        public class DeleteCommentCommand : IRequest<Result<Unit>>
        {
            public Guid CommentId { get; set; }
        }

        public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public DeleteCommentHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var comment = await _context.Comments.FindAsync(request.CommentId);

                if (comment == null || comment.AuthorId != user.Id)
                    return Result<Unit>.Failure("Not found");

                _context.Comments.Remove(comment);
                var success = await _context.SaveChangesAsync() > 0;
                return success
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Not deleted Comment");

            }
        }

    }
}
