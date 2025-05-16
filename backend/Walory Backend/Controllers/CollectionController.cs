using Application.CQRS.Collection;
using Application.DTO;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("collection")]
    public class CollectionsController : BaseApiController
    {



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCollection.CreateCollectionCommand command)
        {
            var result = await Mediator.Send(command);
            return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CollectionDTO collectionDTO)
        {
            var result = await Mediator.Send(new UpdateCollection.Command { CollectionDTO = collectionDTO});
            return result.isSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteCollection.DeleteCollectionCommand { CollectionId = id });
            return result.isSuccess ? NoContent() : BadRequest(result.Error);
        }

        //    [HttpGet]
        //    public async Task<IActionResult> GetAll([FromQuery] GetCollections.Query query)
        //    {
        //        var result = await Mediator.Send(query);
        //        return Ok(result);
        //    }
        //Robimy oddzielnie
    }

}
