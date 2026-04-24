namespace CarRental.Application.Features;

public sealed record RegisterRequestDto(
    string UserName,
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? Address,
    string? Phone);

public sealed record LoginRequestDto(string Email, string Password);

public sealed record AuthResponseDto(string Token, string Role, int UserId, string UserName);
