using Application.CQRS.Account;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.CQRS.Account.ChangeName;
using static Application.CQRS.Account.UpdateDescription;

namespace Walory_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    public class UserController : BaseApiController
    {

        

        [HttpPut("change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeNameCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var result = await Mediator.Send(new DeleteUser.DeleteUserCommand());
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }
        [HttpGet("description")]
        public async Task<IActionResult> GetDescription()
        {
            var result = await Mediator.Send(new GetDescription.GetDescriptionUserQuery());
            return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpPut("description")]
        public async Task<IActionResult> GetDescription([FromBody] UpdateDescriptionCommand command )
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
        [HttpGet("username")]
        public IActionResult GetUsername()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return NotFound("Nie znaleziono nazwy użytkownika.");

            return Ok(username);
        }

    }

}
