using Application.CQRS.FriendsLogic;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    public class FriendController : BaseApiController
    {
        [HttpPost("friends/send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] string email)
        {
            var result = await Mediator.Send(new SendFriendRequest.Command { ReceiverEmail = email });
            if (result.isSuccess) return Ok();
            return BadRequest(result.Error);
        }
        [HttpGet("friends/list")]
        public async Task<IActionResult> AcceptFriendRequest()
        {
            var result = await Mediator.Send(new GetFriendsList.Query());
            if (result.isSuccess) return Ok(result.Value);
            return BadRequest(result.Error);
        }

        [HttpPost("friends/accept/{requestId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        {
            var result = await Mediator.Send(new AcceptFriendRequest.Command { RequestId = requestId });
            if (result.isSuccess) return Ok(result);
            return BadRequest(result.Error);
        }
        [HttpDelete("friends/remove/{friendId}")]
        public async Task<IActionResult> Remove(Guid friendId)
        {
            var result = await Mediator.Send(new RemoveFriend.Command { FriendId = friendId });
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }
    }
}
