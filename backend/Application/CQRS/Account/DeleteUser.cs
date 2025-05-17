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
    public class DeleteUser
    {
        public class DeleteUserCommand : IRequest<Result<Unit>> { }

        public class Handler : IRequestHandler<DeleteUserCommand, Result<Unit>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _http;

            public Handler(UserManager<User> userManager, IHttpContextAccessor http)
            {
                _userManager = userManager;
                _http = http;
            }

            public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                if (user == null) return Result<Unit>.Failure("Użytkownik nie znaleziony");

                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Nie udało się usunąć konta");
            }
        }
    }

}
