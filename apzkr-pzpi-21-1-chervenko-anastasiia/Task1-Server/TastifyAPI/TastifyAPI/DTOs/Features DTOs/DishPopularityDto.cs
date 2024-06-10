using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class DishPopularityDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "OrdersCount is required")]
        [Range(0, int.MaxValue, ErrorMessage = "OrdersCount must be non-negative")]
        public int? OrdersCount { get; set; }
    }
}
