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
    public class ChangeName
    {
        public class ChangeNameCommand : IRequest<Result<Unit>>
        {
            public string NewName { get; set; }
        }

        public class Handler : IRequestHandler<ChangeNameCommand, Result<Unit>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(UserManager<User> userManager, IHttpContextAccessor http)
            {
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<Unit>> Handle(ChangeNameCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                if (user == null) return Result<Unit>.Failure("Użytkownik nie znaleziony");

                user.UserName = request.NewName;
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Nie udało się zmienić nazwy");
            }
        }
    }

}
