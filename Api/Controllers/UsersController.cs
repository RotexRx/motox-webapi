using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers(
            [FromServices] IUserRepository repo,
            [FromQuery] int? count,
            CancellationToken ct)
        {
            var users = await repo.GetAllAsync(count, ct);
            var response = new ApiResponse<List<UserDto>>
            {
                Success = true,
                Data = users
            };
            return Ok(response);
        }

        [HttpPost("suspend/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuspendUser([FromServices] IUserRepository repo, [FromRoute] string Id, CancellationToken ct)
        {
            var suspend = await repo.SuspendUser(Id, ct);
            if (suspend == false)
            {
                return BadRequest(new ApiResponse<object> { Success = true, Message = "خطا در مسدود سازی" });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "کاربر با موفقیت مسدود شد" });
        }

        [HttpPost("unsuspend/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnSuspendUser([FromServices] IUserRepository repo, [FromRoute] string Id, CancellationToken ct)
        {
            var suspend = await repo.UnSuspendUser(Id, ct);
            if (suspend == false)
            {
                return BadRequest(new ApiResponse<object> { Success = true, Message = "خطا در رفع مسدود سازی" });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "کاربر با موفقیت رفع مسدودیت شد" });
        }
    }
}
