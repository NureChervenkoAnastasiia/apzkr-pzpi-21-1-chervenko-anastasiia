using System;
using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class OrderDto
    {
        public string? Id { get; set; }

        public int? Number { get; set; }

        [Required(ErrorMessage = "TableId is required")]
        public string? TableId { get; set; }

        [Required(ErrorMessage = "OrderDateTime is required")]
        public DateTime OrderDateTime { get; set; }

        [StringLength(500, ErrorMessage = "Comment must be at most 500 characters")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }
    }
}
