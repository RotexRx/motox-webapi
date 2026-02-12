using Application.Features.Comments.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICommentRepository _repo;

        public CommentsController(IMediator mediator, ICommentRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }


        [HttpPost("comment")]
        [Authorize]
        public async Task<IActionResult> NewComment([FromBody] CommentCommand command)
        {
            var id = await _mediator.Send(command);
            var response = new ApiResponse<int>
            {
                Success = true,
                Message = "سوال شما ارسال شد",
                Data = id
            };
            return Ok(response);
        }

        [HttpGet("comment/{Id}")]
        [Authorize]
        public async Task<IActionResult> GetComments([FromServices] ICommentRepository repo, [FromRoute] int Id, CancellationToken ct)
        {
            var ad = await repo.GetCommentsAsync(Id, ct);
            var response = new ApiResponse<List<CommentDto>>
            {
                Success = true,
                Data = ad
            };
            return Ok(response);
        }

        [HttpGet("comments")]
        [Authorize]
        public async Task<IActionResult> GetAllComments([FromServices] ICommentRepository repo, CancellationToken ct)
        {
            var ad = await repo.GetAllCommentsAsync(ct);
            var response = new ApiResponse<List<CommentDto>>
            {
                Success = true,
                Data = ad
            };
            return Ok(response);
        }

        [HttpPost("reply/{id}/{reply}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReplyComment(int id, string reply, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "اطلاعات نامعتبر است" });

            var result = await _repo.Reply(id, reply, ct);

            if (result)
            {
                return Ok(new { Success = true, Message = "آگهی با موفقیت پاسخ داده شد" });
            }

            return BadRequest(new { Success = false, Message = "آگهی یافت نشد یا عملیات شکست خورد" });
        }

        [HttpDelete("comment/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "اطلاعات نامعتبر است" });

            var result = await _repo.Delete(id, ct);
            if (result)
            {
                return Ok(new { Success = true, Message = "آگهی با موفقیت حذف شد" });
            }

            return BadRequest(new { Success = false, Message = "آگهی یافت نشد یا عملیات شکست خورد" });
        }

    }
}
