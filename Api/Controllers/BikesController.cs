using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Bikes.Commands.CreateBike;
using Application.Features.Bikes.Queries.GetLatestBikes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoX.Application.Features.Bikes.Queries.GetLatestBikes;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BikesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestBikes([FromQuery] int count = 5)
    {
        var bikes = await _mediator.Send(new GetBikesQuery(count));

        var response = new ApiResponse<List<BikeDto>>
        {
            Success = true,
            Data = bikes
        };

        return Ok(response);
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateBike([FromBody] CreateBikeCommand command)
    {
        var id = await _mediator.Send(command);

        var response = new ApiResponse<int>
        {
            Success = true,
            Message = "موتور با موفقیت ساخته شد",
            Data = id
        };

        return Ok(response);
    }

}


