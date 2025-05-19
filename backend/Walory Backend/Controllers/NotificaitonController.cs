using Application.CQRS;
using Application.CQRS.FriendsLogic;
using Application.CQRS.Notification;
using Application.DTO;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Walory_Backend.Controllers
{
    public class NotificaitonController :BaseApiController
    {

        [HttpGet("notifcation")]
        public async Task<ActionResult<List<NotificationDto>>> GetUsers()
        {
            var result = await Mediator.Send(new NotificationRequest.Query());
            if (result.isSuccess) return Ok(result);
            return BadRequest(result.Error);
        }
        [HttpPost("notifcation/mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await Mediator.Send(new MarkNotificationAsRead.Command { NotificationId = id });
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
        [HttpDelete("notifcation/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteNotification.Command { NotificationId = id });
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
    }

}
