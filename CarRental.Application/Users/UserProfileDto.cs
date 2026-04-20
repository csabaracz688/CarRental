namespace CarRental.Application.Users;

public class UserProfileDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public int PostalCode { get; set; }
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
}