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
        public class CreateWalorTemplateCommand : IRequest<Result<Guid>>
        {
            public string Category { get; set; }
            public JsonDocument Content { get; set; }
            public Visibility Visibility { get; set; }
        }

        public class Handler : IRequestHandler<CreateWalorTemplateCommand, Result<Guid>>
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

            public async Task<Result<Guid>> Handle(CreateWalorTemplateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var template = new WalorTemplate
                {
                    Category = request.Category,
                    Content = request.Content,
                    Visibility = request.Visibility,
                    AuthorId = user.Id
                };

                _context.Templates.Add(template);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(template.Id);
            }
        }
    }
}
