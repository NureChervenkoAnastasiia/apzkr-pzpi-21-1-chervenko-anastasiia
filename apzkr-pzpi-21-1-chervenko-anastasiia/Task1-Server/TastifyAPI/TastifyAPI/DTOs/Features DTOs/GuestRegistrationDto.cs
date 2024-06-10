using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class GuestRegistrationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Mobile number must contain exactly 12 digits")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*\d).*$", ErrorMessage = "Password must contain at least one numeric digit")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Login is required")]
        [RegularExpression(@"^[a-zA-Z0-9]{3,20}$", ErrorMessage = "Login must be at least 3 characters long and " +
            "contain only alphanumeric characters")]
        public string? Login {  get; set; }
    }
}
