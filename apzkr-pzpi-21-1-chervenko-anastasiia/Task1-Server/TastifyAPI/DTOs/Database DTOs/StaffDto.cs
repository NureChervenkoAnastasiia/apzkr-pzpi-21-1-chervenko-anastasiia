using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class StaffDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Position must be between 1 and 100 characters")]
        public string? Position { get; set; }

        [Required(ErrorMessage = "HourlySalary is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "HourlySalary must be greater than 0")]
        public double? HourlySalary { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Mobile number must contain exactly 12 digits")]
        public long? Phone { get; set; }

        [Required(ErrorMessage = "AttendanceCard is required")]
        [Range(1, int.MaxValue, ErrorMessage = "AttendanceCard must be greater than 0")]
        public int? AttendanceCard { get; set; }

        [Required(ErrorMessage = "Login is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Login must be between 5 and 50 characters")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "PasswordHash must be between 6 and 20 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "RestaurantId is required")]
        public string? RestaurantId { get; set; }
    }
}
