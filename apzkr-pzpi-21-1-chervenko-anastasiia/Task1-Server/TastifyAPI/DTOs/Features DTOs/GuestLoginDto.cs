using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class GuestLoginDto
    {
        [Required(ErrorMessage = "Login is required")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
