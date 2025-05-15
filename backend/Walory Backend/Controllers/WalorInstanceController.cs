using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalorInstancesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalorInstancesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalorInstance.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalorInstance.Command command)
        {
            if (id != command.WalorInstanceId)
                return BadRequest("ID mismatch.");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteWalorInstance.Command { WalorInstanceId = id });
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetWalorInstanceById.Query { WalorInstanceId = id });
            return result != null ? Ok(result) : NotFound();
        }
    }

}
