using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;
    public int Amount { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
