using Application.CQRS.Account;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.CQRS.Account.ChangeName;

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
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
    }

}
