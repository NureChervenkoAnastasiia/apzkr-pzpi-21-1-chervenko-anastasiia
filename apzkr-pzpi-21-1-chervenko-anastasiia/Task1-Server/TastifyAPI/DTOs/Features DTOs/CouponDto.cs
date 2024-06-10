using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class CouponDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Bonus must be non-negative")]
        public int Bonus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be non-negative")]
        public decimal? Discount { get; set; }
    }
}
