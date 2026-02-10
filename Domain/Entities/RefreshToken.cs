using MotoX.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = default!;

    public DateTime Expires { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;

    public DateTime Created { get; set; }

    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}
