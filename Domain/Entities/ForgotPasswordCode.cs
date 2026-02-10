using MotoX.Domain.Entities;

public class ForgotPasswordCode
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;  
    public string UserId { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool Used { get; set; } = false;

    public ApplicationUser User { get; set; } = null!;
}
