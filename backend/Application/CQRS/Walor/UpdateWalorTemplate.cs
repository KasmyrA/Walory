using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class UpdateWalorTemplate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _context;
            private readonly IHttpContextAccessor _http;

            public Handler(AppDbContext context, IHttpContextAccessor http)
            {
                _context = context;
                _http = http;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _http.HttpContext.User.GetUserId();
                var template = await _context.WalorTemplates.FindAsync(request.TemplateId);

                if (template == null || template.AuthorId != userId)
                    return Result<Unit>.Failure("Szablon nie istnieje lub brak dostępu.");

                template.Category = request.Category;
                template.Content = request.Content;
                template.Visibility = request.Visibility;

                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
