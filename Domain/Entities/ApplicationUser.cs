using Microsoft.AspNetCore.Identity;

namespace MotoX.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Suspended { get; set; } = false;
}
