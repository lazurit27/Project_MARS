using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class CreateRepairBoxDto
    {
        [Required(ErrorMessage = "Box number is required.")]
        [MaxLength(50, ErrorMessage = "Box number cannot exceed 50 characters.")]
        public string BoxNumber { get; set; } = string.Empty;

        [Range(0.01, 1000000.00, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "AutoService ID is required.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
    }
    public class CreateRapairBoxDto
    {
    }
}
