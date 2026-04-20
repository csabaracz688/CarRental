using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Application.Features
{
    public class RequestRentalDto
    {
        public int CarId { get; set; }
        public int? UserId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
