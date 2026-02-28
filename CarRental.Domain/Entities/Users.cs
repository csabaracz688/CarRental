using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Entities;

public class Users
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Rental> ApprovedRent { get; set; } = new List<Rental>();
}
