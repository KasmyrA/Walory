using Domain;
using Infrastracture;
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
    public class GetAvatar
    {
        public class GetAvatarQuery : IRequest<byte[]?>
        {
           
        }
        public class GetAvatarHandler : IRequestHandler<GetAvatarQuery, byte[]?>
        {
            private readonly DataContext _context;
            private readonly IHttpContextAccessor _http;
            private readonly UserManager<User> _userManager;

            public GetAvatarHandler(DataContext context, IHttpContextAccessor http, UserManager<User> userManager)
            {
                _context = context;
                _http = http;
                _userManager = userManager;
            }

            public async Task<byte[]?> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);
                return user?.AvatarImage;
            }
        }

    }
}
