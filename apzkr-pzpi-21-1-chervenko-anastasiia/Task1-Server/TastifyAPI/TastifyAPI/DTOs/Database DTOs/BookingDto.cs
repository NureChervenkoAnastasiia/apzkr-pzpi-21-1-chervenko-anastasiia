using System;
using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class BookingDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "TableId is required")]
        public string? TableId { get; set; }

        [Required(ErrorMessage = "GuestId is required")]
        public string? GuestId { get; set; }

        [Required(ErrorMessage = "BookingDateTime is required")]
        public DateTime BookingDateTime { get; set; }

        [Required(ErrorMessage = "PersonsCount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "PersonsCount must be greater than 0")]
        public int? PersonsCount { get; set; }

        [StringLength(500, ErrorMessage = "Comment must be at most 500 characters")]
        public string? Comment { get; set; }
    }
}
