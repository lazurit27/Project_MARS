using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class UpdateRepairBoxDto
    {
        [Required(ErrorMessage = "Box number is required.")]
        [MaxLength(50, ErrorMessage = "Box number cannot exceed 50 characters.")]
        public string BoxNumber { get; set; } = string.Empty;

        [Range(0.00, 1000000.00, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
    }
}
