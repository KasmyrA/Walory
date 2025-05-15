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
    public class CreateWalorTemplate
    {
        public class Command : IRequest<Result<Guid>>
        {
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly AppDbContext _context;
            private readonly IHttpContextAccessor _http;

            public Handler(AppDbContext context, IHttpContextAccessor http)
            {
                _context = context;
                _http = http;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _http.HttpContext.User.GetUserId();

                var template = new WalorTemplate
                {
                    Id = Guid.NewGuid(),
                    Category = request.Category,
                    Content = request.Content,
                    Visibility = request.Visibility,
                    AuthorId = userId
                };

                _context.WalorTemplates.Add(template);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(template.Id);
            }
        }
    }
}
