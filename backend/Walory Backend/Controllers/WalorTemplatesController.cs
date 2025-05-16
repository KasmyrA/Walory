using Application.CQRS.Walor;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("WalorTemplates")]
    public class WalorTemplatesController : BaseApiController
    {


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalorTemplate.Command command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalorTemplate.Command command)
        {
            if (id != command.TemplateId)
                return BadRequest("ID mismatch.");

            var result = await Mediator.Send(command);
            return result.isSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteWalorTemplate.Command { TemplateId = id });
            return result.isSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpPost("{id}/import")]
        public async Task<IActionResult> Import(Guid id)
        {
            var result = await Mediator.Send(new CopyWalorTemplate.Command { TemplateId = id });
            return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }

}
