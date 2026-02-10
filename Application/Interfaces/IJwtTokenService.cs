using MotoX.Domain.Entities;

public interface IJwtTokenService
{
    Task<(string accessToken, string refreshToken)> GenerateTokens(ApplicationUser user);
    Task<string> RefreshAccessToken(string refreshToken);
}
