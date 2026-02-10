using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoX.Domain.Entities;

public class EmailVerificationCode
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public DateTime Expires { get; set; }

    public bool Used { get; set; } = false;
     
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}