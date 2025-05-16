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
        public class Command : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                var template = await _context.Templates.FindAsync(request.TemplateId);

                if (template == null || template.AuthorId != user.Id)
                    return Result<Unit>.Failure("Template not found");

                template.Category = request.Category;
                template.Content = request.Content;
                template.Visibility = request.Visibility;

                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
