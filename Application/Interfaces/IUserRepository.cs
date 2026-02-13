
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository

{
    
    Task<List<UserDto>> GetAllAsync(int? Count,CancellationToken cancellationToken);
    Task<bool> SuspendUser(string Id, CancellationToken cancellationToken);
    Task<bool> UnSuspendUser(string Id, CancellationToken cancellationToken);

}
