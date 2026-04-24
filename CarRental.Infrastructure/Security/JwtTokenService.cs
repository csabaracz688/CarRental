using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(Users user, RoleTypes role)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "CarRental.Api";
        var audience = jwtSection["Audience"] ?? "CarRental.Frontend";
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

        if (key.Length < 16)
        {
            throw new InvalidOperationException("JWT key length must be at least 16 characters.");
        }

        var expirationMinutes = 120;
        if (int.TryParse(jwtSection["AccessTokenExpirationMinutes"], out var configuredMinutes) && configuredMinutes > 0)
        {
            expirationMinutes = configuredMinutes;
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
