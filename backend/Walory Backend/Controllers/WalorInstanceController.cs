using Application.CQRS.Walor;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalorInstancesController : BaseApiController
    {


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalorInstance.CreateWalorInstanceCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateWalorInstance.UpdateWalorInstanceCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteWalorInstance.DeleteWalorInstanceCommand { WalorInstanceId = id });
            return result.isSuccess ? Ok(result) : BadRequest(result.Error);
        }

    }

}
