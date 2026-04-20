using CarRental.Application.Users;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Managers;

public class UserManager : IUserManager
{
    private readonly CarRentalDbContext _context;

    public UserManager(CarRentalDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null) return null;

        return new UserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Address = user.Address,
            Phone = user.Phone
        };
    }

    public async Task UpdateProfileAsync(int id, UpdateUserProfileDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new Exception("User not found");

        user.Address = dto.Address;
        user.Phone = dto.Phone;

        await _context.SaveChangesAsync();
    }
}