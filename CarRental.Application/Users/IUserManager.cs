namespace CarRental.Application.Users;

public interface IUserManager
{
    Task<UserProfileDto> GetByIdAsync(int id);
    Task UpdateProfileAsync(int id, UpdateUserProfileDto dto);
}