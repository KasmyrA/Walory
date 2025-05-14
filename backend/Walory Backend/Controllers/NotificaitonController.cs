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
            if (result.isSuccess) return Ok();
            return BadRequest(result.Error);
        }
    }

}
