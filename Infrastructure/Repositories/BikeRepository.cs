using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Bikes.Commands.CreateBike;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Repositories;

public class BikeRepository : IBikeRepository
{
    private readonly AppDbContext _context;

    public BikeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bike>> GetBikesAsync(int count)
    {
        return await _context.Bikes
            .OrderByDescending(b => b.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> CreateBikesAsync(Bike bike,CancellationToken cancellationToken)
    {
        if (bike == null)
        {
            return false; 
        }

        await _context.Bikes.AddAsync(bike);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
