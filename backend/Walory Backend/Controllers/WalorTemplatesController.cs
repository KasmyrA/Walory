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
        public async Task<IActionResult> Create([FromBody] CreateWalorTemplate.CreateWalorTemplateCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateWalorTemplate.UpdateWalorTemplateCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteWalorTemplate.DeleteWalorTemplateCommand { TemplateId = id });
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPost("{id}/import")]
        public async Task<IActionResult> Import(Guid id)
        {
            var result = await Mediator.Send(new CopyWalorTemplate.CopyWalorTemplateCommand { TemplateId = id });
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }
    }

}
