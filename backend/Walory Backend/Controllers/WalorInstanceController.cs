using Application.CQRS.Collection;
using Application.CQRS.Walor;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.CQRS.Account.UploadAvatar.UploadAvatarHandler;

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

        [HttpPut]
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
        [HttpPost("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(Guid id, [FromForm] UploadAvatarDto dto)
        {
            var result = await Mediator.Send(new UploadWalorImageCommand { WalorId = id, ImageFile = dto.Avatar });
            return result.isSuccess ? Ok() : BadRequest(result.Error);
        }
        [HttpGet("{id}/image")]
        public async Task<IActionResult> Get(Guid id)
        {
            var imageBytes = await Mediator.Send(new GetWalorImageQuery { WalorId = id });
            if (imageBytes == null || imageBytes.Length == 0)
                return NotFound();

            return File(imageBytes, "image/jpeg");
        }
    }

}
