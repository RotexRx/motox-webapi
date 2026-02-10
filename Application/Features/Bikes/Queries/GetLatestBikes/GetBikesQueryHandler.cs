using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using MotoX.Application.Features.Bikes.Queries.GetLatestBikes;

namespace Application.Features.Bikes.Queries.GetLatestBikes;

public class GetBikesQueryHandler : IRequestHandler<GetBikesQuery, List<BikeDto>>
{
    private readonly IBikeRepository _repository;

    public GetBikesQueryHandler(IBikeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BikeDto>> Handle(GetBikesQuery request, CancellationToken cancellationToken)
    {
        var bikes = await _repository.GetBikesAsync(request.Count);

        return bikes.Select(b => new BikeDto
        {
            Id = b.Id,
            Category = b.Category,
            Title = b.Title,
            Subtitle = b.Subtitle,
            Price = b.Price,
            Image = b.Image
        }).ToList();
    }
}
