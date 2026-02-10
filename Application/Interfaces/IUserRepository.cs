
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository

{

    Task<List<UserDto>> GetAllAsync(int? Count,CancellationToken cancellationToken);

}
