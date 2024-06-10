using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class MenuDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "RestaurantId is required")]
        public string? RestaurantId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Size must be greater than 0")]
        public int Size { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int? Price { get; set; }

        [StringLength(200, ErrorMessage = "Info must be at most 200 characters")]
        public string? Info { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }
    }
}
