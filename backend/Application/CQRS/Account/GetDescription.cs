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
    public class GetDescription
    {
        public class GetDescriptionUserQuery: IRequest<Result<string>> { }

        public class Handler : IRequestHandler<GetDescriptionUserQuery, Result<string>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(UserManager<User> userManager, IHttpContextAccessor http)
            {
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<string>> Handle(GetDescriptionUserQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                if (user == null) return Result<string>.Failure("User not found");

                return Result<string>.Success(user.Description);
            }
        }
    }
}
