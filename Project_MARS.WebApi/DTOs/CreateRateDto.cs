using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class CreateRateDto
    {
        [Required(ErrorMessage = "Rating is required.")]
        [Range(1.0, 5.0, ErrorMessage = "Rating must be between 1.0 and 5.0.")]
        public double Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "AutoService ID is required.")]
        public int ServiceId { get; set; }
    }
}
