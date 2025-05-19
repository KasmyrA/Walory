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

namespace Application.CQRS.Account
{
    public class UpdateDescription
    {
        public class UpdateDescriptionCommand : IRequest<Result<Unit>> {
            public string Description { get; set; }
        }

        public class Handler : IRequestHandler<UpdateDescriptionCommand, Result<Unit>>
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
        

            public async Task<Result<Unit>> Handle(UpdateDescriptionCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                if (user == null) return Result<Unit>.Failure("User not found");

                var state = await _context.Users.FindAsync(user.Id);
                if (state == null) return Result<Unit>.Failure("User not found");
                state.Description = request.Description;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
            }
        }
    }
}
