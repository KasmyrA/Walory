using Application.CQRS.Collection;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("collection")]
    public class CollectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CollectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCollection.CreateCollectionCommand command)
        {
            var result = await _mediator.Send(command);
            return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCollection.Command command)
        {
            if (id != command.CollectionId)
                return BadRequest("ID mismatch.");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCollection.Command { CollectionId = id });
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCollections.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}
