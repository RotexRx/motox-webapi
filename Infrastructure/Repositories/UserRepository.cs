using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
         
        public async Task<List<UserDto>> GetAllAsync(int? Count, CancellationToken cancellationToken)
        {
            var query = _context.Users.Where(w => w.Id != "" ).AsQueryable();

            if (Count.HasValue)
                query = query.Take(Count.Value);

            var users = await query.ToListAsync(cancellationToken);

            return users.Select(b => new UserDto
            {
                Id = b.Id,
                LockoutEnabled  = b.LockoutEnabled,
                CreatedAt = b.CreatedAt,
                Email = b.Email,
                EmailConfirmed = b.EmailConfirmed,
                NormalizedEmail = b.NormalizedEmail,
                NormalizedUserName = b.NormalizedUserName,
                PhoneNumber = b.PhoneNumber,
                PhoneNumberConfirmed = b.PhoneNumberConfirmed,
                TwoFactorEnabled    = b.TwoFactorEnabled,
                UserName = b.UserName,
            }).ToList();
        }

     
    }
}
