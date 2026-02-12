using Domain.Entities;
using Domain.Entities.Comments;
using Domain.Entities.Contact;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Bike> Bikes => Set<Bike>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ForgotPasswordCode> ForgotPasswordCodes => Set<ForgotPasswordCode>();
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    public DbSet<Advertisement> Advertisements { get; set; } = default!;
    public DbSet<AdvertisementImage> AdvertisementImages { get; set; } = default!;
    public DbSet<Contact> Contacts { get; set; } = default!;
    public DbSet<VehicleHistory> VehicleHistory { get; set; } = default!;
    public DbSet<Comment> Comments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>()
            .ToTable("RefreshTokens");  
    }

}
