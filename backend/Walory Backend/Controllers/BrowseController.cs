using Application.CQRS.Show;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrowseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrowseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("collections/public")]
        public async Task<IActionResult> GetPublicCollections([FromQuery] GetPublicCollections.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("collections/friends")]
        public async Task<IActionResult> GetFriendsCollections([FromQuery] GetFriendsCollections.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("templates/accessible")]
        public async Task<IActionResult> GetAccessibleTemplates([FromQuery] GetAccessibleTemplates.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}
