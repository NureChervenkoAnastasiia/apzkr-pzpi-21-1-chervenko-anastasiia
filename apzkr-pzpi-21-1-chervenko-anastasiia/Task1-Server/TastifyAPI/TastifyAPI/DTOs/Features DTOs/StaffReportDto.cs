using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class StaffReportDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "TotalWorkingHours must be non-negative")]
        public double? TotalWorkingHours { get; set; }
    }
}
