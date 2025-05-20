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
    public class CreateWalorTemplate
    {
        public class CreateWalorTemplateCommand : IRequest<Result<Unit>>
        {
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<CreateWalorTemplateCommand, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(CreateWalorTemplateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var template = new WalorTemplate
                {
                    Category = request.Category,
                    Content = EnsureParsedJson(request.Content),
                    Visibility = request.Visibility,
                    AuthorId = user.Id
                };

                _context.Templates.Add(template);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
            }
        }
        private static JsonDocument EnsureParsedJson(JsonDocument doc)
        {
            if (doc.RootElement.ValueKind == JsonValueKind.String)
            {
                var rawJson = doc.RootElement.GetString();
                return JsonDocument.Parse(rawJson!);
            }

            return doc;
        }
    }
}
