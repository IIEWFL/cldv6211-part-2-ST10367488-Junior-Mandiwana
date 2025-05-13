using System;
using System.Collections.Generic;

namespace EventEase_System.Models;

public partial class VwBookingDetail
{
    public int BookingId { get; set; }

    
    public string? EventName { get; set; }

    public DateTime? EventDate { get; set; }  

    public string? Description { get; set; }

    public string? VenueName { get; set; }

    public string? Location { get; set; }

    public int? Capacity { get; set; }  

    public DateTime? BookingDate { get; set; }  
}

