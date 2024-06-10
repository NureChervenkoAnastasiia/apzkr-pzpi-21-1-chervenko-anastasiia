using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class RestaurantDto
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ0-9\s,.'-]+$", ErrorMessage = "Invalid address format")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Info must be at most 200 characters")]
        public string? Info { get; set; }

        [Required(ErrorMessage = "At least one cuisine is required")]
        public List<string>? Cuisine { get; set; }
    }
}
