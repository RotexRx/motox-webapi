using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity; 
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MotoX.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;  

    public JwtTokenService(IConfiguration config, AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _config = config;
        _db = db;
        _userManager = userManager;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokens(ApplicationUser user)
    {
        var accessToken = await GenerateJwt(user);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var entity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _db.RefreshTokens.Add(entity);
        await _db.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    private async Task<string> GenerateJwt(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> RefreshAccessToken(string refreshToken)
    {
        var tokenEntity = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (tokenEntity == null || tokenEntity.IsExpired)
            throw new SecurityTokenException("Invalid refresh token");

        _db.RefreshTokens.Remove(tokenEntity);
        await _db.SaveChangesAsync();

        return await GenerateJwt(tokenEntity.User);
    }
}