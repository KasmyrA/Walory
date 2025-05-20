using Application.DTO;
using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Collection
{
    public class CreateCollection
    {
        public class CreateCollectionCommand : IRequest<Result<Unit>>
        {
          public CollectionDTO CollectionDTO { get; set; }
        }

        public class CreateCollectionCommandHandler : IRequestHandler<CreateCollectionCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;

            private readonly IHttpContextAccessor _httpContextAccessor;

            public CreateCollectionCommandHandler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == request.CollectionDTO.WalorTemplateId);
                if (template == null || template.AuthorId != user.Id) {  return Result<Unit>.Failure("Failed to create collection"); }
                var collection = new Domain.Collection()
                {
                    Title = request.CollectionDTO.Title,
                    Description = request.CollectionDTO.Description,
                    Visibility = request.CollectionDTO.Visibility,
                    WalorTemplateId = request.CollectionDTO.WalorTemplateId,
                    OwnerId = user.Id,
                    Owner = user,
                };

                _context.Collections.Add(collection);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Failed to create collection");
                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}
