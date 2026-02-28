using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public RoleTypes RoleType { get; set; }

    public ICollection<Users> Users { get; set; } = new List<Users>();
}
