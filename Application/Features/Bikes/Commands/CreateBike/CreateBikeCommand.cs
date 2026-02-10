using MediatR;

namespace Application.Features.Bikes.Commands.CreateBike;

public class CreateBikeCommand : IRequest<int>
{
    public string Category { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Subtitle { get; set; } = default!;
    public string Price { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string Badge { get; set; } = default!;
}
