using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class TableDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number must be greater than 0")]
        public int? Number { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }
    }
}
