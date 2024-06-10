using System;
using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class ScheduleDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "StaffId is required")]
        public string? StaffId { get; set; }

        [Required(ErrorMessage = "StartDateTime is required")]
        public DateTime? StartDateTime { get; set; }

        [Required(ErrorMessage = "FinishDateTime is required")]
        public DateTime? FinishDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDateTime >= FinishDateTime)
            {
                yield return new ValidationResult("FinishDateTime must be greater than StartDateTime", new[] { nameof(FinishDateTime) });
            }
        }
    }
}

