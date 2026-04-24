using CarRental.Domain.Entities;
using CarRental.Domain.Enums;

namespace CarRental.Infrastructure.Security;

public interface IJwtTokenService
{
    string CreateToken(Users user, RoleTypes role);
}
