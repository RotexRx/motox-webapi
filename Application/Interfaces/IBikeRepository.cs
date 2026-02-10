using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Bikes.Commands.CreateBike;
using MotoX.Domain.Entities;

namespace Application.Interfaces;

public interface IBikeRepository
{
    Task<List<Bike>> GetBikesAsync(int count);
    Task<bool> CreateBikesAsync(Bike bike,CancellationToken cancellationToken);
}
