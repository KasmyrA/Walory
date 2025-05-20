using Domain;
using MediatR;
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
            public string UserId { get; set; } = default!;
        }
        public class GetAvatarHandler : IRequestHandler<GetAvatarQuery, byte[]?>
        {
            private readonly UserManager<User> _userManager;

            public GetAvatarHandler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<byte[]?> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                return user?.AvatarImage;
            }
        }

    }
}
