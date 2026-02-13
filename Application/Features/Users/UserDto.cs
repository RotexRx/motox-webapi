using Microsoft.AspNetCore.Identity;

public class UserDto : IdentityUser
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Suspended { get; set; } = false;
}
