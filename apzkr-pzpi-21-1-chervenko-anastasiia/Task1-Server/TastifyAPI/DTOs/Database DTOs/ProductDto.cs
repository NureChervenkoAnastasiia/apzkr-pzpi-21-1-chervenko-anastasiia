using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class ProductDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 150 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public double? Amount { get; set; }
    }
}
