using Application.DTO;
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

namespace Application.CQRS.Collection
{
    public class UpdateCollection
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; } 
            public string Title { get; set; }
            public string Description { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;

            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var collection = await _context.Collections.FindAsync(request.CollectionId);

                if (collection == null || collection.OwnerId != user.Id)
                    return Result<Unit>.Failure("Collestion not found");

                collection.Title = request.Title;
                collection.Description = request.Description;
                collection.Visibility = request.Visibility;


                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Failed to updatew collection");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
