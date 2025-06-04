using Application.CQRS.Account;
using Application.CQRS.Chat;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : BaseApiController
    {


        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMessage.Command command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpGet("messages/{friendId}")]
        public async Task<IActionResult> GetMessages(Guid friendId)
        {
            var result = await Mediator.Send(new GetMessages.Query { FriendId = friendId });
            return Ok(result);
        }
        [HttpPost("mark-as-read/{friendId}")]
        public async Task<IActionResult> MarkAsRead(Guid friendId)
        {
            var result = await Mediator.Send(new MarkMessagesAsRead.Command { FriendId = friendId });
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
        [HttpGet("getID")]
        public async Task<IActionResult> getID(Guid friendId)
        {
            var result = await Mediator.Send(new getID.getIDQuery());
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPost("remove-message-notifications/{friendId}")]
        public async Task<IActionResult> RemoveMessageNotifications(Guid friendId)
        {
            var result = await Mediator.Send(new RemoveMessageNotifications.Command { FriendId = friendId });
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }

    }

}
