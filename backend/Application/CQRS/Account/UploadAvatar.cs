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
    public class UploadAvatar
    {
        public class UploadAvatarCommand : IRequest<Result<Unit>>
        {
            public IFormFile AvatarFile { get; set; } = default!;
        }
        public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, Result<Unit>>
        {
            private readonly UserManager<User> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UploadAvatarHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Unit>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
                if (user == null)
                    return Result<Unit>.Failure("User not found");

                if (request.AvatarFile == null || request.AvatarFile.Length == 0)
                    return Result<Unit>.Failure("Invalid file");

                using var memoryStream = new MemoryStream();
                await request.AvatarFile.CopyToAsync(memoryStream);

                var avatarBytes = memoryStream.ToArray();

                user.AvatarImage = avatarBytes;

                var updateResult = await _userManager.UpdateAsync(user);

                return updateResult.Succeeded ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update avatar");
            }
        }

    }
}
