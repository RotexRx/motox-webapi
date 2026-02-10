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
    }
}
