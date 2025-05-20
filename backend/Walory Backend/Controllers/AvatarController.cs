using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Application.CQRS.Account.GetAvatar;
using static Application.CQRS.Account.UploadAvatar;
using static Application.CQRS.Account.UploadAvatar.UploadAvatarHandler;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvatarController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public AvatarController(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarDto dto)
        {
            var command = new UploadAvatarCommand { AvatarFile = dto.Avatar };
            var result = await _mediator.Send(command);
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAvatar(string userId)
        {
            var avatarBytes = await _mediator.Send(new GetAvatarQuery { UserId = userId });
            if (avatarBytes == null || avatarBytes.Length == 0)
                return NotFound();

            return File(avatarBytes, "image/jpeg");
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetMyAvatar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var avatarBytes = user.AvatarImage;
            if (avatarBytes == null || avatarBytes.Length == 0)
                return NotFound();

            return File(avatarBytes, "image/jpeg");
        }
    }

}
