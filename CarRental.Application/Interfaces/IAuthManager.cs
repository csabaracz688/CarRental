using CarRental.Application.Features;

namespace CarRental.Application.Common.Interfaces;

public interface IAuthManager
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
}
