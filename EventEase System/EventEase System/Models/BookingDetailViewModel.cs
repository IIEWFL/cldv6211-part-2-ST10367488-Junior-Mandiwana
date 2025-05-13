using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase_System.Models
{
    public class BookingDetailViewModel
    {
        public string? EventName { get; set; }

        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public int Capacity { get; set; }
        public DateTime BookingDate { get; set; }

        public int EventId { get; set; }
        public int VenueId { get; set; }
        

        public IFormFile? SupportingFile { get; set; }

        public string? SupportingFileUrl { get; set; }

    }
}
