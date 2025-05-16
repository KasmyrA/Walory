using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class DeleteWalorTemplate
    {
        public class DeleteWalorTemplateCommand : IRequest<Result<Unit>>
        {
            public Guid TemplateId { get; set; }
        }

        public class Handler : IRequestHandler<DeleteWalorTemplateCommand, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(DeleteWalorTemplateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                var template = await _context.Templates
                    .FirstOrDefaultAsync(t => t.Id == request.TemplateId);

                if (template == null || template.AuthorId != user.Id)
                    return Result<Unit>.Failure("Template Not Found");

                if (_context.Collections.Where(t => t.WalorTemplateId == request.TemplateId).Any())
                    return Result<Unit>.Failure("Template used by any collection");

                _context.Templates.Remove(template);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

}
