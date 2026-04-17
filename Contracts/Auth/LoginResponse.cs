namespace sispark_api.Contracts.Auth;

public record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    string FullName,
    string Username);
