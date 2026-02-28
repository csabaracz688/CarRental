using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Enums;

public enum CarRentStatus
{
    Requested = 0,
    Approved = 1,
    Rejected = 2,
    Handed = 3,
    Returned = 4
}
