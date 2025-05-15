using Application.CQRS.Walor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("WalorTemplates")]
    public class WalorTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalorTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalorTemplate.Command command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalorTemplate.Command command)
        {
            if (id != command.TemplateId)
                return BadRequest("ID mismatch.");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteWalorTemplate.Command { TemplateId = id });
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpPost("{id}/import")]
        public async Task<IActionResult> Import(Guid id)
        {
            var result = await _mediator.Send(new ImportWalorTemplate.Command { TemplateId = id });
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }

}
