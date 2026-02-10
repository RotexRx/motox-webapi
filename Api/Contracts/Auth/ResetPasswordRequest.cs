namespace Api.Contracts.Auth;

public record ResetPasswordRequest(
    string Code,
    string NewPassword);
