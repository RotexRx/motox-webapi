namespace Api.Contracts.Auth;

public record RegisterRequest(
    string Username,
    string Number,
    string Email,
    string Password);
