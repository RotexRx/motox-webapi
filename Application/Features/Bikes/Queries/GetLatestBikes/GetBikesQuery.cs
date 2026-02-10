using MediatR;
using System.Collections.Generic;

namespace MotoX.Application.Features.Bikes.Queries.GetLatestBikes;

public record GetBikesQuery(int Count) : IRequest<List<BikeDto>>;
