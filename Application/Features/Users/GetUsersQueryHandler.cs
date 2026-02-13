using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Queries.GetLatestBikes;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var ads = await _repository.GetAllAsync(request.Count, cancellationToken);

        return ads.Select(b => new UserDto
        {
            Id = b.Id,
            LockoutEnabled = b.LockoutEnabled,
            CreatedAt = b.CreatedAt,
            Email = b.Email,
            EmailConfirmed = b.EmailConfirmed,
            NormalizedEmail = b.NormalizedEmail,
            NormalizedUserName = b.NormalizedUserName,
            PhoneNumber = b.PhoneNumber,
            PhoneNumberConfirmed = b.PhoneNumberConfirmed,
            TwoFactorEnabled = b.TwoFactorEnabled,
            UserName = b.UserName,
        }).ToList();
    }
}
