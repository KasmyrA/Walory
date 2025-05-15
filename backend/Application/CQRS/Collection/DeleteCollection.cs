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

namespace Application.CQRS.Collection
{
    public class DeleteCollection
    {
        public class DeleteCollectionCommand : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
        }

        public class DeleteCollectionCommandHandler : IRequestHandler<DeleteCollectionCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public DeleteCollectionCommandHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                var collection = await _context.Collections
                    .Include(c => c.Walors)
                    .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

                if (collection == null || collection.OwnerId != user.Id)
                    return Result<Unit>.Failure("Not found");

                _context.Walors.RemoveRange(collection.Walors);
                _context.Collections.Remove(collection);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete notification");

                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}
