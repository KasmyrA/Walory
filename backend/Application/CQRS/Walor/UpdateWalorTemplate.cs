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
    public class UpdateWalorTemplate
    {
        public class UpdateWalorTemplateCommand : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<UpdateWalorTemplateCommand, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(UpdateWalorTemplateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                var template = await _context.Templates.FindAsync(request.TemplateId);

                if (template == null || template.AuthorId != user.Id)
                    return Result<Unit>.Failure("Template not found");

                template.Category = request.Category;
                template.Content = EnsureParsedJson(request.Content);
                template.Visibility = request.Visibility;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
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

}
