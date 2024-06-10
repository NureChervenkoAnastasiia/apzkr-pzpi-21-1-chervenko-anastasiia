using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class StaffLoginDto
    {
        [Required(ErrorMessage = "Login is required")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
