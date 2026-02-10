using MediatR;
using Microsoft.AspNetCore.Mvc;
using MotoX.Application.Features.Home.Queries.GetHomeInfo;

namespace MotoX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetHomeInfoQuery());
        return Ok(result);
    }
}
