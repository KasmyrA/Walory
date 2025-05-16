using Application.CQRS.Show;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrowseController : BaseApiController
    {

        [HttpGet("collections/public")]
        public async Task<IActionResult> GetPublicCollections([FromQuery] GetPublicCollections.Query query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("collections/friends")]
        public async Task<IActionResult> GetFriendsCollections([FromQuery] GetFriendsCollections.Query query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("templates/accessible")]
        public async Task<IActionResult> GetAccessibleTemplates([FromQuery] GetAccessibleTemplates.Query query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("templates/private")]
        public async Task<IActionResult> GetPrivateTemplates([FromQuery] GetPrivateTemplates.Query query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("collections/private")]
        public async Task<IActionResult> GetPrivateCollections([FromQuery] GetPrivateCollections.Query query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }

}
