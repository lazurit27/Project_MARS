using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class AddCarDto
    {
        [Required(ErrorMessage = "VIN is required.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be exactly 17 characters.")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Invalid VIN format.")]
        public string Vin { get; set; } = string.Empty;
    }
}
