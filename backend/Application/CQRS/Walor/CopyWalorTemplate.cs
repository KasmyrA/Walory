using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class CopyWalorTemplate
    {
        public class CopyWalorTemplateCommand : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
        }

        public class Handler : IRequestHandler<CopyWalorTemplateCommand, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor http)
            {
                _context = context;
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<Unit>> Handle(CopyWalorTemplateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                var original = await _context.Templates.FindAsync(request.TemplateId);

                if (original == null) return Result<Unit>.Failure("Not found");

                if (original.Visibility == Visibility.Private)
                    return Result<Unit>.Failure("Failure"); //Nie chcemy mowic ze jest prywatne

                var copy = new WalorTemplate
                {
                    Id = Guid.NewGuid(),
                    AuthorId = user.Id,
                    Category = original.Category,
                    Content = JsonDocument.Parse(original.Content.RootElement.GetRawText()),
                    Visibility = Visibility.Private
                };

                _context.Templates.Add(copy);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
            }
        }
    }

}
