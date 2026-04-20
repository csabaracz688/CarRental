using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Enums;

public enum CarUnavailableReason
{
    Maintenance = 1, 
    Accident = 2, 
    Repair = 3,      
    Cleaning = 4,    
    AdminHold = 5,   
    Other = 6
}
