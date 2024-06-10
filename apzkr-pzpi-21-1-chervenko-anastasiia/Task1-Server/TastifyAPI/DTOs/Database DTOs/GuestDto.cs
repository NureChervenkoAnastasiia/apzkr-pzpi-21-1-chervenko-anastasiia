using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class GuestDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Mobile number must contain exactly 12 digits")]
        public string? Phone { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Bonus must be a positive number")]
        public Int32 Bonus { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        public string? Password { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]{5,20}$", ErrorMessage = "Login must be between 5 and 20 characters long, containing only letters and numbers.")]
        public string? Login { get; set; }


    }
}
