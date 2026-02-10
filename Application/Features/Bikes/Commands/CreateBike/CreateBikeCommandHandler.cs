using Application.Interfaces;
using MediatR;
using MotoX.Application.Features.Bikes.Queries.GetLatestBikes;
using MotoX.Domain.Entities;

namespace Application.Features.Bikes.Commands.CreateBike;

public class CreateBikeCommandHandler
    : IRequestHandler<CreateBikeCommand, int>
{

    private readonly IBikeRepository _repository;

    public CreateBikeCommandHandler(IBikeRepository repository)
    {
        _repository = repository;
    }


    public async Task<int> Handle(CreateBikeCommand request, CancellationToken cancellationToken)
    {
        var bike = new Bike
        {
            Category = request.Category,
            Title = request.Title,
            Subtitle = request.Subtitle,
            Price = request.Price,
            Image = request.Image,
            Badge = request.Badge
        };

        await _repository.CreateBikesAsync(bike, cancellationToken);
         

        return bike.Id;
    }
 
}
