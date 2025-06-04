using Application.DTO;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Account
{
    public class getID
    {
        public class getIDQuery : IRequest<Result<Guid>> { }

        public class Handler : IRequestHandler<getIDQuery, Result<Guid>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(UserManager<User> userManager, IHttpContextAccessor http)
            {
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<Guid>> Handle(getIDQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                if (user == null) return Result<Guid>.Failure("User not found");
                return Result<Guid>.Success(user.Id);
            }
        }
    }
}
