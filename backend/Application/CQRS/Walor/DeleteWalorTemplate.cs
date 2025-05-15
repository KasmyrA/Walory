using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class DeleteWalorTemplate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
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
                var template = await _context.WalorTemplates
                    .Include(t => t.Collections)
                    .FirstOrDefaultAsync(t => t.Id == request.TemplateId);

                if (template == null || template.AuthorId != userId)
                    return Result<Unit>.Failure("Szablon nie istnieje lub brak dostępu.");

                if (template.Collections.Any())
                    return Result<Unit>.Failure("Nie można usunąć szablonu, który jest używany przez kolekcje.");

                _context.WalorTemplates.Remove(template);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
