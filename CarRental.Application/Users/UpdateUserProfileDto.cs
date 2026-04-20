namespace CarRental.Application.Users;

public class UpdateUserProfileDto
{

    public int PostalCode { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}