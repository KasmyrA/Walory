using Application.CQRS.LikeAndSubscribe;
using Cars.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Walory_Backend.Security;
using static Application.CQRS.LikeAndSubscribe.AddComment;
using static Application.CQRS.LikeAndSubscribe.AddLike;
using static Application.CQRS.LikeAndSubscribe.ChecIfLiked;
using static Application.CQRS.LikeAndSubscribe.DeleteCommand;
using static Application.CQRS.LikeAndSubscribe.GetComments;
using static Application.CQRS.LikeAndSubscribe.GetLikes;

namespace Walory_Backend.Controllers
{
   [RequireConfirmedEmail]
    public class CollectionIteractionController
    {
        [ApiController]
        [Route("api/collections/{collectionId}/interactions")]
        public class CollectionInteractionsController : BaseApiController
        {

            [HttpPost("comments")]
            public async Task<IActionResult> AddComment(Guid collectionId, [FromBody] AddCommentCommand command)
            {
                command.CollectionId = collectionId;
                var result = await Mediator.Send(command);
                return result.isSuccess ? Ok(result.Value) : BadRequest(result.Error);
            }

            [HttpDelete("comments/{commentId}")]
            public async Task<IActionResult> DeleteComment(Guid collectionId, Guid commentId)
            {
                var command = new DeleteCommentCommand { CommentId = commentId };
                var result = await Mediator.Send(command);
                return result.isSuccess ? Ok() : BadRequest(result.Error);
            }

            [HttpPost("likes")]
            public async Task<IActionResult> AddLike(Guid collectionId)
            {
                var command = new AddLikeCommand { CollectionId = collectionId };
                var result = await Mediator.Send(command);
                return result.isSuccess ? Ok() : BadRequest(result.Error);
            }

            [HttpDelete("likes")]
            public async Task<IActionResult> RemoveLike(Guid collectionId)
            {
                var command = new RemoveLikeCommand { CollectionId = collectionId };
                var result = await Mediator.Send(command);
                return result.isSuccess ? Ok() : BadRequest(result.Error);
            }

            [HttpGet("comments")]
            public async Task<IActionResult> GetComments(Guid collectionId)
            {
                var query = new GetCommentsQuery { CollectionId = collectionId };
                var comments = await Mediator.Send(query);
                return Ok(comments);
            }

            [HttpGet("likes/count")]
            public async Task<IActionResult> GetLikesCount(Guid collectionId)
            {
                var query = new GetLikesCountQuery { CollectionId = collectionId };
                var count = await Mediator.Send(query);
                return Ok(count);
            }
            [HttpGet("check-if-liked")]
            public async Task<IActionResult> Check_if_liked(Guid collectionId)
            {
                var query = new ChecIfLikedCommand { CollectionId = collectionId };
                var count = await Mediator.Send(query);
                return Ok(count);
            }
        }

    }
}
